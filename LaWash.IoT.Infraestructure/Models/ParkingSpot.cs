namespace LaWash.IoT.Infraestructure;

public class ParkingSpot
{

    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ParkingName { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? WasDeletedAt { get; set; }

    public ICollection<ParkingSpotsDevice> ParkingSpotsDevices { get; set; } = new List<ParkingSpotsDevice>();
    public ICollection<ParkingSpotsStatus> ParkingSpotsStatuses { get; set; } = new List<ParkingSpotsStatus>();
}

