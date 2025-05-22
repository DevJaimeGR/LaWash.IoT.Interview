namespace LaWash.IoT.Infraestructure;

public class Device
{
    public required string Id { get; set; }
    public string DeviceSpecification { get; set; } =string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? WasDeletedAt { get; set; }
    public bool IsWorking { get; set; } = true; //we should use this field for monitoring if the device is working or not 
    public DateTime RegisterAt { get; set; } = DateTime.UtcNow;
    public ICollection<ParkingSpotsDevice> ParkingSpotsDevices { get; set; } = new List<ParkingSpotsDevice>();
}