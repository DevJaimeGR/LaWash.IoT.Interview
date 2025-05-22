using LaWash.IoT.Domain;
using LaWash.IoT.Infraestructure;
using LaWash.IoT.Transversal;

namespace LaWash.IoT.Application;

public class ParkingApplication : IParkingApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRateLimiter _rateLimiter;

    public ParkingApplication(IUnitOfWork unitOfWork, IRateLimiter rateLimiter)
    {
        _unitOfWork = unitOfWork;
        _rateLimiter = rateLimiter;
    }

    public async Task<PagedResult<GetAllDevicesOutputDTO>> GetAllDevices(PaginationParams paginationParams)
    {

        var devices = await _unitOfWork.Devices.GetPagedAsync(x => x.IsWorking == true && !x.IsDeleted ,paginationParams);

        List<GetAllDevicesOutputDTO> deviceOutputDTOs = new();

        foreach (var item in devices.Items)
        {
            var parkingSpotsDevices = await _unitOfWork.ParkingSpotsDevices.FindAsync(x=> x.DeviceId == item.Id.ToString());

            if (parkingSpotsDevices != null)
            {
                deviceOutputDTOs.Add( new GetAllDevicesOutputDTO() { DeviceId = item.Id, Status = "Registered and in use" });
            }
            else
            {
                deviceOutputDTOs.Add(new GetAllDevicesOutputDTO() { DeviceId = item.Id, Status = "Registered but not in use" });
            }
        }

        var pagedResult = new PagedResult<GetAllDevicesOutputDTO>
        {
            Items = deviceOutputDTOs,
            TotalCount = devices.TotalCount,
            PageNumber = devices.PageNumber,
            PageSize = devices.PageSize
        };

        return pagedResult;
    }

    public async Task Occupy(Guid deviceId)
    {
        if(!_rateLimiter.IsRequestAllowed(deviceId))
            throw new BadResponseWithMessage("Rate limit exceeded", (int) Enums.StatusCode.TooManyRequests);

        var device = await _unitOfWork.Devices.FindAsync(x => x.Id == deviceId.ToString() && !x.IsDeleted);

        if (device == null)
            throw new BadResponseWithMessage("Invalid device", (int)Enums.StatusCode.NotFound);

        var parkingSpot = await _unitOfWork.ParkingSpotsDevices.FindAsync(x => x.DeviceId == deviceId.ToString() && !x.IsDeleted);

        if (parkingSpot == null)
            throw new BadResponseWithMessage("Device not register", (int)Enums.StatusCode.NotFound);

        var parkingSpotStatus = await _unitOfWork.ParkingSpotsStatus.FindAsync(x => x.ParkingSpotId == parkingSpot.ParkingSpotId.ToString() && !x.IsDeleted);

        if (parkingSpotStatus == null)
            throw new BadResponseWithMessage("Parking spot status not found", (int)Enums.StatusCode.NotFound);

        if (parkingSpotStatus.Status)
            throw new BadResponseWithMessage("Parking spot already occupied", (int)Enums.StatusCode.Conflict);

        parkingSpotStatus.Status = true;
        await _unitOfWork.ParkingSpotsStatus.UpdateAsync(parkingSpotStatus);
    }

    public async Task Free(Guid deviceId)
    {
        if (!_rateLimiter.IsRequestAllowed(deviceId))
            throw new BadResponseWithMessage("Rate limit exceeded", (int)Enums.StatusCode.TooManyRequests);
        var device = await _unitOfWork.Devices.FindAsync(x => x.Id == deviceId.ToString() && !x.IsDeleted);

        if (device == null)
            throw new BadResponseWithMessage("Invalid device", (int)Enums.StatusCode.NotFound);

        var parkingSpot = await _unitOfWork.ParkingSpotsDevices.FindAsync(x => x.DeviceId == deviceId.ToString() && !x.IsDeleted);

        if (parkingSpot == null)
            throw new BadResponseWithMessage("Device not register", (int)Enums.StatusCode.NotFound);

        var parkingSpotStatus = await _unitOfWork.ParkingSpotsStatus.FindAsync(x => x.ParkingSpotId == parkingSpot.ParkingSpotId.ToString() && !x.IsDeleted);

        if (parkingSpotStatus == null)
            throw new BadResponseWithMessage("Parking spot status not found", (int)Enums.StatusCode.NotFound);

        if (!parkingSpotStatus.Status)
            throw new BadResponseWithMessage("Parking spot already free", (int)Enums.StatusCode.Conflict);

        parkingSpotStatus.Status = false;
        await _unitOfWork.ParkingSpotsStatus.UpdateAsync(parkingSpotStatus);   
    }

    public async Task<PagedResult<GetSpotsOutputDTO>> GetParkingSpotsStatus(PaginationParams paginationParams)
    {
        var allSpots = await _unitOfWork.ParkingSpots.FindAllAsync(x => !x.IsDeleted);

        if (allSpots == null)
            throw new BadResponseWithMessage("Parking spots not found", (int)Enums.StatusCode.NotFound);

        var totalCount = allSpots.Count();

        var pagedSpots = allSpots
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        List<GetSpotsOutputDTO> spotsOutputDTOs = new();

        foreach (var item in pagedSpots)
        {
            var parkingSpotStatus = await _unitOfWork.ParkingSpotsStatus.FindAsync(x => x.ParkingSpotId == item.Id.ToString() && !x.IsDeleted);

            if (parkingSpotStatus != null)
            {
                spotsOutputDTOs.Add(new GetSpotsOutputDTO
                {
                    ParkingSpotId = item.Id.ToString(),
                    ParkingName = item.ParkingName,
                    Status = parkingSpotStatus.Status ? "Occupied" : "Free"
                });
            }
        }

        var pagedResult = new PagedResult<GetSpotsOutputDTO>
        {
            Items = spotsOutputDTOs,
            TotalCount = totalCount,
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize
        };

        return pagedResult;
    }

    public async Task DeleteSpot(Guid parkingSpotId)
    {
        var parkingSpot = await _unitOfWork.ParkingSpots.FindAsync(x => x.Id == parkingSpotId.ToString() && !x.IsDeleted);

        if (parkingSpot == null)
            throw new BadResponseWithMessage("Parking spot not found", (int)Enums.StatusCode.NotFound);

        var parkingSpotStatus = await _unitOfWork.ParkingSpotsStatus.FindAsync(x => x.ParkingSpotId == parkingSpotId.ToString() && !x.IsDeleted);

        if (parkingSpotStatus == null)
            throw new BadResponseWithMessage("Parking spot status not found", (int)Enums.StatusCode.NotFound);

        var parkingSpotsDevices = await _unitOfWork.ParkingSpotsDevices.FindAsync(x => x.ParkingSpotId == parkingSpotId.ToString() && !x.IsDeleted);

        if (parkingSpotsDevices == null)
            throw new BadResponseWithMessage("Parking spot-device not found", (int)Enums.StatusCode.NotFound);

        var device = await _unitOfWork.Devices.FindAsync(x => x.Id == parkingSpotsDevices.DeviceId.ToString() && !x.IsDeleted);
        if (device == null)
            throw new BadResponseWithMessage("Device not found", (int)Enums.StatusCode.NotFound);

        device.IsDeleted = true;
        device.WasDeletedAt = DateTime.UtcNow;
        parkingSpot.IsDeleted = true;
        parkingSpot.WasDeletedAt = DateTime.UtcNow;
        parkingSpotStatus.IsDeleted = true;
        parkingSpotStatus.WasDeletedAt = DateTime.UtcNow;
        parkingSpotsDevices.IsDeleted = true;
        parkingSpotsDevices.WasDeletedAt = DateTime.UtcNow;

        await _unitOfWork.Devices.UpdateAsync(device);
        await _unitOfWork.ParkingSpots.UpdateAsync(parkingSpot);
        await _unitOfWork.ParkingSpotsStatus.UpdateAsync(parkingSpotStatus);
        await _unitOfWork.ParkingSpots.UpdateAsync(parkingSpot);
    }

    public async Task<CreateSpotOutputDTO> CreateParkingSpot(CreateSpotInputDTO spotsInputDTO)
    {
        var parkingSpot = await _unitOfWork.ParkingSpots.FindNoTrackingAsync(x => x.Id == spotsInputDTO.ParkingSpotId.ToString() && !x.IsDeleted);
        var device = await _unitOfWork.Devices.FindNoTrackingAsync(x => x.Id == spotsInputDTO.DeviceId.ToString() && !x.IsDeleted);

        int selectStrategy = GetStrategy(parkingSpot, device);

        var CreateSpotStrategy = new CreateParkingSpot(
            new SpotAndDeviceDoNotExistStrategy(_unitOfWork),
            new SpotDoesNotExistAndDeviceExistsStrategy(_unitOfWork),
            new SpotExistsAndDeviceDoesNotExistStrategy(_unitOfWork),
            new SpotAndDeviceExistsStrategy());

        var strategyResult = await CreateSpotStrategy.CreateSpotAsync(selectStrategy, spotsInputDTO);
        
        return strategyResult;
    }

    private int GetStrategy(object? parkingSpot, object? device)
    {
        return (parkingSpot, device) switch
        {
            (null, null) => (int)Enums.Strategy.None,
            (null, not null) => (int)Enums.Strategy.OnlyDevice,
            (not null, null) => (int)Enums.Strategy.OnlyParkingSpot,
            (not null, not null) => (int)Enums.Strategy.Both
        };
    }

}

