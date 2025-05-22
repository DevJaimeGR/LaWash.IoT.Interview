using Microsoft.EntityFrameworkCore;

namespace LaWash.IoT.Infraestructure;

public class ParkingDbContext : DbContext
{
    public ParkingDbContext(DbContextOptions<ParkingDbContext> options) : base(options)
    {
    }

    public DbSet<ParkingSpot> ParkingSpots { get; set; } = null!;
    public DbSet<Device> Devices { get; set; } = null!;
    public DbSet<ParkingSpotsDevice> ParkingSpotsDevices { get; set; } = null!;
    public DbSet<ParkingSpotsStatus> ParkingSpotsStatus { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Device>()
            .HasMany(d => d.ParkingSpotsDevices)
            .WithOne(psd => psd.Device)
            .HasForeignKey(psd => psd.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ParkingSpot>()
            .HasMany(p => p.ParkingSpotsDevices)
            .WithOne(psd => psd.ParkingSpot)
            .HasForeignKey(psd => psd.ParkingSpotId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ParkingSpot>()
            .HasMany(p => p.ParkingSpotsStatuses)
            .WithOne(pss => pss.ParkingSpot)
            .HasForeignKey(pss => pss.ParkingSpotId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Device>().HasKey(d => d.Id);
        modelBuilder.Entity<ParkingSpot>().HasKey(p => p.Id);
        modelBuilder.Entity<ParkingSpotsDevice>().HasKey(psd => psd.Id);
        modelBuilder.Entity<ParkingSpotsStatus>().HasKey(pss => pss.Id);
    }
}

