using LaWash.IoT.Domain;
using LaWash.IoT.Infraestructure;

namespace LaWash.IoT.Application;

public class SpotAndDeviceDoNotExistStrategy : ICreateParkingSpotStrategy
{
    private readonly IUnitOfWork _unitOfWork;
    public SpotAndDeviceDoNotExistStrategy(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<CreateSpotOutputDTO> CreateSpotAsync(CreateSpotInputDTO input)
    {
        await _unitOfWork.ParkingSpots.AddAsync(new ParkingSpot
        {
            Id = input.ParkingSpotId.ToString(),
            ParkingName = input.ParkingName,
        });

        await _unitOfWork.Devices.AddAsync(new Device
        {
            Id = input.DeviceId.ToString(),
            DeviceSpecification = input.DeviceSpecification
        });

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

        return new CreateSpotOutputDTO
        {
            Message = "Parking spot created successfully with new device",
            ParkingSpotId = input.ParkingSpotId.ToString(),
            DeviceId = input.DeviceId.ToString(),
        };
    }
}

