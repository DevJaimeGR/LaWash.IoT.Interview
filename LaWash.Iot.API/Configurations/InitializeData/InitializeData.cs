using LaWash.IoT.Domain;
using LaWash.IoT.Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace LaWash.Iot.API;
/// <summary>
/// Initializes data for the application.
/// </summary>
public class InitializerData
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializerData"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work instance.</param>
    public InitializerData(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    /// <summary>
    /// Starts the initialization process asynchronously.
    /// </summary>
    /// <param name="serviceProvider">The service provider instance.</param>
    public async Task StartAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ParkingDbContext>();

        if (await context.Devices.AnyAsync()) return;

        #region Devices
        var devices = new List<Device>
        {
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-1"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-2"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-3"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-4"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-5"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-6"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-7"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-8"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-9"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-10"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-11"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-12"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-13"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-14"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-15"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-16"},
            new Device { Id = Guid.NewGuid().ToString(), DeviceSpecification = "Raspberry Pi 5-17"},
        };
        #endregion

        #region ParkingSpots
        var parkingSpots = new List<ParkingSpot>
        {
            new ParkingSpot { ParkingName = "Spot 1"},
            new ParkingSpot { ParkingName = "Spot 2"},
            new ParkingSpot { ParkingName = "Spot 3"},
            new ParkingSpot { ParkingName = "Spot 4"},
            new ParkingSpot { ParkingName = "Spot 5"},
            new ParkingSpot { ParkingName = "Spot 6"},
            new ParkingSpot { ParkingName = "Spot 7"},
            new ParkingSpot { ParkingName = "Spot 8"},
            new ParkingSpot { ParkingName = "Spot 9"},
            new ParkingSpot { ParkingName = "Spot 10"}
        };
        #endregion

        #region ParkingSpotsDevices
        var parkingSpotsDevices = new List<ParkingSpotsDevice>
        {
            new ParkingSpotsDevice { DeviceId = devices[0].Id, ParkingSpotId = parkingSpots[0].Id },
            new ParkingSpotsDevice { DeviceId = devices[1].Id, ParkingSpotId = parkingSpots[1].Id },
            new ParkingSpotsDevice { DeviceId = devices[2].Id, ParkingSpotId = parkingSpots[2].Id },
            new ParkingSpotsDevice { DeviceId = devices[3].Id, ParkingSpotId = parkingSpots[3].Id },
            new ParkingSpotsDevice { DeviceId = devices[4].Id, ParkingSpotId = parkingSpots[4].Id },
            new ParkingSpotsDevice { DeviceId = devices[5].Id, ParkingSpotId = parkingSpots[5].Id },
            new ParkingSpotsDevice { DeviceId = devices[6].Id, ParkingSpotId = parkingSpots[6].Id },
            new ParkingSpotsDevice { DeviceId = devices[7].Id, ParkingSpotId = parkingSpots[7].Id },
            new ParkingSpotsDevice { DeviceId = devices[8].Id, ParkingSpotId = parkingSpots[8].Id },
            new ParkingSpotsDevice { DeviceId = devices[9].Id, ParkingSpotId = parkingSpots[9].Id }
        };
        #endregion

        #region ParkingSpotsStatus
        var parkingSpotsStatuses = new List<ParkingSpotsStatus>
        {
            new ParkingSpotsStatus { ParkingSpotId = parkingSpots[0].Id, Status = true },
            new ParkingSpotsStatus { ParkingSpotId = parkingSpots[1].Id, Status = false },
            new ParkingSpotsStatus { ParkingSpotId = parkingSpots[2].Id, Status = true },
            new ParkingSpotsStatus { ParkingSpotId = parkingSpots[3].Id, Status = false },
            new ParkingSpotsStatus { ParkingSpotId = parkingSpots[4].Id, Status = true },
            new ParkingSpotsStatus { ParkingSpotId = parkingSpots[5].Id, Status = false },
            new ParkingSpotsStatus { ParkingSpotId = parkingSpots[6].Id, Status = true },
            new ParkingSpotsStatus { ParkingSpotId = parkingSpots[7].Id, Status = false },
            new ParkingSpotsStatus { ParkingSpotId = parkingSpots[8].Id, Status = true },
            new ParkingSpotsStatus { ParkingSpotId = parkingSpots[9].Id, Status = false }
        };
        #endregion

        await _unitOfWork.Devices.AddRangeAsync(devices);
        await _unitOfWork.ParkingSpots.AddRangeAsync(parkingSpots);
        await _unitOfWork.ParkingSpotsDevices.AddRangeAsync(parkingSpotsDevices);
        await _unitOfWork.ParkingSpotsStatus.AddRangeAsync(parkingSpotsStatuses);
    }
}

