using LaWash.IoT.Domain;
using LaWash.IoT.Infraestructure;
using LaWash.IoT.Transversal;

namespace LaWash.IoT.Application;

public class SpotDoesNotExistAndDeviceExistsStrategy : ICreateParkingSpotStrategy
{
    private readonly IUnitOfWork _unitOfWork;

    public SpotDoesNotExistAndDeviceExistsStrategy(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<CreateSpotOutputDTO> CreateSpotAsync(CreateSpotInputDTO input)
    {
        var parkingSpot = await _unitOfWork.ParkingSpotsDevices.FindNoTrackingAsync(x => x.DeviceId == input.DeviceId.ToString());

        if (parkingSpot != null)
            throw new BadResponseWithMessage("Device already in use with other parking spot", (int)Enums.StatusCode.Conflict);

        await _unitOfWork.ParkingSpots.AddAsync(new ParkingSpot
        {
            Id = input.ParkingSpotId.ToString(),
            ParkingName = input.ParkingName,
        });

        await _unitOfWork.ParkingSpotsStatus.AddAsync(new ParkingSpotsStatus
        {
            Id = Guid.NewGuid().ToString(),
            ParkingSpotId = input.ParkingSpotId.ToString(),
            Status = false
        });
        await _unitOfWork.ParkingSpotsDevices.AddAsync(new ParkingSpotsDevice
        {
            Id = Guid.NewGuid().ToString(),
            DeviceId = input.DeviceId.ToString(),
            ParkingSpotId = input.ParkingSpotId.ToString()
        });

        return new CreateSpotOutputDTO()
        {
            ParkingSpotId = input.ParkingSpotId.ToString(),
            DeviceId = input.DeviceId.ToString(),
            Message = $"The existing device has already been registered to the new parking spot: '{input.ParkingName}'."
        };
    }
}
