using LaWash.IoT.Infraestructure;

namespace LaWash.IoT.Application;

public interface IParkingApplication
{
    Task Occupy(Guid deviceId);
    Task Free(Guid deviceId);
    Task<PagedResult<GetAllDevicesOutputDTO>> GetAllDevices(PaginationParams paginationParams);
    Task<PagedResult<GetSpotsOutputDTO>> GetParkingSpotsStatus(PaginationParams paginationParams);
    Task<CreateSpotOutputDTO> CreateParkingSpot(CreateSpotInputDTO spotsInputDTO);
    Task DeleteSpot(Guid parkingSpotId);
}

