using LaWash.IoT.Domain;
using LaWash.IoT.Transversal;

namespace LaWash.IoT.Application;
public class SpotAndDeviceExistsStrategy : ICreateParkingSpotStrategy
{

    public Task<CreateSpotOutputDTO> CreateSpotAsync(CreateSpotInputDTO input)
    {
        throw new BadResponseWithMessage("Device and parking spot is already in use", (int) Enums.StatusCode.Conflict);
    }
}

