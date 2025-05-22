namespace LaWash.IoT.Application;
public class CreateSpotOutputDTO
{
    public required string Message { get; set; } = string.Empty;
    public required string ParkingSpotId { get; set; } = string.Empty;
    public required string DeviceId { get; set; } = string.Empty;
}

