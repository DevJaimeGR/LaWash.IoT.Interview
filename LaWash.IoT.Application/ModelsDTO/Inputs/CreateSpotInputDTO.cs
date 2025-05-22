namespace LaWash.IoT.Application;
public class CreateSpotInputDTO
{
    public Guid ParkingSpotId { get; set; }
    public string ParkingName { get; set; } = string.Empty;
    public Guid DeviceId { get; set; }
    public string DeviceSpecification { get; set; } = string.Empty;
}

