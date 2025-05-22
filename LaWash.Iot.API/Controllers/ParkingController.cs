using LaWash.IoT.Application;
using LaWash.IoT.Infraestructure;
using LaWash.IoT.Transversal;
using Microsoft.AspNetCore.Mvc;

namespace LaWash.Iot.API.Controllers;

/// <summary>
/// Controller for managing parking-related operations.
/// </summary>
[ApiController]
[Route(ApiRoutes.Base)]
public class ParkingController : Controller
{
    private readonly IParkingApplication _parkingApplication;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParkingController"/> class.
    /// </summary>
    /// <param name="parkingApplication">The parking application service.</param>
    public ParkingController(IParkingApplication parkingApplication)
    {
        _parkingApplication = parkingApplication;
    }

    /// <summary>
    /// Retrieves a paginated list of all registered IoT devices.
    /// </summary>
    /// <param name="paginationParams">Pagination parameters (page, size, etc.).</param>
    /// <returns>A paginated list of IoT devices.</returns>
    [HttpGet]
    [Route(ApiRoutes.GetAllDevices)]
    [Produces(ContentType.Json)]
    [ProducesResponseType(typeof(IEnumerable<PagedResult<GetAllDevicesOutputDTO>>), (int)Enums.StatusCode.Ok)]
    [ProducesResponseType(typeof(ErrorResponseWithMessage), (int)Enums.StatusCode.BadRequest)]
    public async Task<IActionResult> GetAllDevices([FromQuery] PaginationParams paginationParams)
    {
        var devices = await _parkingApplication.GetAllDevices(paginationParams);
        return Ok(devices);
    }

    /// <summary>
    /// Marks a parking spot as occupied by a given IoT device.
    /// </summary>
    /// <param name="id">The ID of the IoT device occupying the spot.</param>
    /// <returns>No content if the operation is successful.</returns>
    [HttpPost]
    [Route(ApiRoutes.PostOccupy)]
    [Produces(ContentType.Json)]
    [ProducesResponseType((int)Enums.StatusCode.NoContent)]
    [ProducesResponseType(typeof(ErrorResponseWithMessage), (int)Enums.StatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorResponseWithMessage), (int)Enums.StatusCode.TooManyRequests)]
    [ProducesResponseType(typeof(ErrorResponseWithMessage), (int)Enums.StatusCode.Conflict)]
    [ProducesResponseType(typeof(ErrorResponseWithMessage), (int)Enums.StatusCode.BadRequest)]
    public async Task<IActionResult> OccupySpot([FromRoute] Guid id)
    {
        await _parkingApplication.Occupy(id);
        return StatusCode((int)Enums.StatusCode.NoContent);
    }

    /// <summary>
    /// Marks a parking spot as free by a given IoT device.
    /// </summary>
    /// <param name="id">The ID of the IoT device freeing the spot.</param>
    /// <returns>No content if the operation is successful.</returns>
    [HttpPost]
    [Route(ApiRoutes.PostFree)]
    [Produces(ContentType.Json)]
    [ProducesResponseType((int)Enums.StatusCode.NoContent)]
    [ProducesResponseType(typeof(ErrorResponseWithMessage), (int)Enums.StatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorResponseWithMessage), (int)Enums.StatusCode.TooManyRequests)]
    [ProducesResponseType(typeof(ErrorResponseWithMessage), (int)Enums.StatusCode.Conflict)]
    [ProducesResponseType(typeof(ErrorResponseWithMessage), (int)Enums.StatusCode.BadRequest)]
    public async Task<IActionResult> FreeSpot([FromRoute] Guid id)
    {
        await _parkingApplication.Free(id);
        return StatusCode((int)Enums.StatusCode.NoContent);
    }

    /// <summary>
    /// Retrieves the current status (occupied or free) of all parking spots.
    /// </summary>
    /// <param name="paginationParams">Pagination parameters.</param>
    /// <returns>A paginated list with the status of each parking spot.</returns>
    [HttpGet]
    [Route(ApiRoutes.GetParkingSpotsStatus)]
    [Produces(ContentType.Json)]
    [ProducesResponseType(typeof(IEnumerable<GetSpotsOutputDTO>), (int)Enums.StatusCode.Ok)]
    [ProducesResponseType(typeof(ErrorResponseWithMessage), (int)Enums.StatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorResponseWithMessage), (int)Enums.StatusCode.BadRequest)]
    public async Task<IActionResult> GetParkingSpotsStatus([FromQuery] PaginationParams paginationParams)
    {
        var parkingSpotsResult = await _parkingApplication.GetParkingSpotsStatus(paginationParams);
        return StatusCode((int)Enums.StatusCode.Ok, parkingSpotsResult);
    }

    /// <summary>
    /// Creates a new parking spot.
    /// </summary>
    /// <param name="createSpotInputDTO">Information required to create the parking spot.</param>
    /// <returns>The created parking spot information.</returns>
    [HttpPost]
    [Route(ApiRoutes.PostCreateSpot)]
    [Produces(ContentType.Json)]
    [ProducesResponseType(typeof(GetSpotsOutputDTO), (int)Enums.StatusCode.Created)]
    [ProducesResponseType(typeof(ErrorResponseWithMessage), (int)Enums.StatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseWithMessage), (int)Enums.StatusCode.Conflict)]
    public async Task<IActionResult> CreateSpot([FromBody] CreateSpotInputDTO createSpotInputDTO)
    {
        var parkingSpotResult = await _parkingApplication.CreateParkingSpot(createSpotInputDTO);
        return StatusCode((int)Enums.StatusCode.Created, parkingSpotResult);
    }

    /// <summary>
    /// Deletes an existing parking spot.
    /// </summary>
    /// <param name="id">The ID of the parking spot to delete.</param>
    /// <returns>No content if deletion is successful.</returns>
    [HttpDelete]
    [Route(ApiRoutes.DeleteSpot)]
    [Produces(ContentType.Json)]
    [ProducesResponseType(typeof(GetSpotsOutputDTO), (int)Enums.StatusCode.NoContent)]
    [ProducesResponseType(typeof(ErrorResponseWithMessage), (int)Enums.StatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorResponseWithMessage), (int)Enums.StatusCode.BadRequest)]
    public async Task<IActionResult> DeleteSpot([FromRoute] Guid id)
    {
        await _parkingApplication.DeleteSpot(id);
        return StatusCode((int)Enums.StatusCode.NoContent);
    }
}
