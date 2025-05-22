using LaWash.IoT.Infraestructure;

namespace LaWash.IoT.Domain;

public class UnitOfWork : IUnitOfWork
{
    private readonly ParkingDbContext _context;

    public IRepository<Device> Devices { get; }
    public IRepository<ParkingSpot> ParkingSpots { get; }
    public IRepository<ParkingSpotsDevice> ParkingSpotsDevices { get; }
    public IRepository<ParkingSpotsStatus> ParkingSpotsStatus { get; }
    public UnitOfWork(ParkingDbContext context)
    {
        _context = context;
        Devices = new Repository<Device>(context);
        ParkingSpots = new Repository<ParkingSpot>(context);
        ParkingSpotsDevices = new Repository<ParkingSpotsDevice>(context);
        ParkingSpotsStatus = new Repository<ParkingSpotsStatus>(context);
    }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public void Dispose()
    {
        _context.Dispose();
    }
}

