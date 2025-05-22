namespace LaWash.IoT.Infraestructure;

public class ParkingSpotsStatus
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public required string ParkingSpotId { get; set; }

    public bool Status { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public DateTime? WasDeletedAt { get; set; }

    public ParkingSpot? ParkingSpot { get; set; }
}

