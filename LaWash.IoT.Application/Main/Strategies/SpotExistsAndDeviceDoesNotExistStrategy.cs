using LaWash.IoT.Domain;
using LaWash.IoT.Infraestructure;
using LaWash.IoT.Transversal;

namespace LaWash.IoT.Application;
public class SpotExistsAndDeviceDoesNotExistStrategy : ICreateParkingSpotStrategy
{
    private readonly IUnitOfWork _unitOfWork;

    public SpotExistsAndDeviceDoesNotExistStrategy(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<CreateSpotOutputDTO> CreateSpotAsync(CreateSpotInputDTO input)
    {
        var parkingSpot = await _unitOfWork.ParkingSpotsDevices.FindNoTrackingAsync(x => x.ParkingSpotId == input.ParkingSpotId.ToString() && !x.IsDeleted);

        if (parkingSpot != null)
            throw new BadResponseWithMessage("Parking spot already in use with other device", (int)Enums.StatusCode.Conflict);

        await _unitOfWork.ParkingSpotsDevices.AddAsync(new ParkingSpotsDevice
        {
            Id = Guid.NewGuid().ToString(),
            DeviceId = input.DeviceId.ToString(),
            ParkingSpotId = input.ParkingSpotId.ToString()
        });

        await _unitOfWork.ParkingSpotsStatus.AddAsync(new ParkingSpotsStatus
        {
            Id = Guid.NewGuid().ToString(),
            ParkingSpotId = input.ParkingSpotId.ToString(),
            Status = false
        });
        await _unitOfWork.Devices.AddAsync(new Device
        {
            Id = input.DeviceId.ToString(),
            DeviceSpecification = input.DeviceSpecification
        });
        return new CreateSpotOutputDTO()
        {
            ParkingSpotId = input.ParkingSpotId.ToString(),
            DeviceId = input.DeviceId.ToString(),
            Message = $"The new device '{input.DeviceSpecification}' has been registered to the existing parking spot: '{input.ParkingName}'."
        };
    }

}

