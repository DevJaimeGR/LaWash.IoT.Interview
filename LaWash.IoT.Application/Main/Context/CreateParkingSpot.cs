using LaWash.IoT.Domain;
using LaWash.IoT.Transversal;
using System.Security.Cryptography.X509Certificates;

namespace LaWash.IoT.Application;

public class CreateParkingSpot
{
    private readonly Dictionary<int, ICreateParkingSpotStrategy> _strategies;

    public CreateParkingSpot(
        SpotAndDeviceDoNotExistStrategy none,
        SpotDoesNotExistAndDeviceExistsStrategy onlyDevice,
        SpotExistsAndDeviceDoesNotExistStrategy onlySpot,
        SpotAndDeviceExistsStrategy both)
    {

        _strategies = new Dictionary<int, ICreateParkingSpotStrategy>
        {
            {(int) Enums.Strategy.None, none },
            {(int) Enums.Strategy.OnlyDevice, onlyDevice },
            {(int) Enums.Strategy.OnlyParkingSpot, onlySpot },
            {(int) Enums.Strategy.Both, both }
        };
    }

    public async Task<CreateSpotOutputDTO> CreateSpotAsync(int SelectedStrategy,CreateSpotInputDTO input)
    {
        if(_strategies.TryGetValue(SelectedStrategy, out var strategy))
        {
            return await strategy.CreateSpotAsync(input);
        }
        else
        {
            throw new ArgumentException("Invalid strategy");
        }
    }
}

