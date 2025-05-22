using LaWash.IoT.Infraestructure;

namespace LaWash.IoT.Domain;

public interface IUnitOfWork : IDisposable
{
    IRepository<Device> Devices { get; }
    IRepository<ParkingSpot> ParkingSpots { get; }
    IRepository<ParkingSpotsDevice> ParkingSpotsDevices { get; }
    IRepository<ParkingSpotsStatus> ParkingSpotsStatus { get; }

    Task<int> CompleteAsync();
}

