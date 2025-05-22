namespace LaWash.IoT.Infraestructure;

public class ParkingSpotsDevice
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public required string ParkingSpotId { get; set; }

    public required string DeviceId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? WasDeletedAt { get; set; }

    public ParkingSpot? ParkingSpot { get; set; }
    public Device? Device { get; set; }
}

