namespace LaWash.IoT.Application;

public interface ICreateParkingSpotStrategy
{
    Task<CreateSpotOutputDTO> CreateSpotAsync(CreateSpotInputDTO input);
}

