using Moq;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using LaWash.IoT.Application;
using LaWash.Iot.API.Controllers;
using LaWash.IoT.Transversal;
using LaWash.IoT.Infraestructure;

namespace LaWash.IoT.API.Tests.Controllers;

public class ParkingControllerTests
{
    private readonly Mock<IParkingApplication> _mockApp;
    private readonly ParkingController _controller;

    public ParkingControllerTests()
    {
        _mockApp = new Mock<IParkingApplication>();
        _controller = new ParkingController(_mockApp.Object);
    }

    [Fact]
    public async Task GetAllDevices_ShouldReturnOkResult_WithDeviceList()
    {
        // Arrange
        var pagination = new PaginationParams { PageNumber = 1, PageSize = 10 };
        var result = new PagedResult<GetAllDevicesOutputDTO>
        {
            Items = new List<GetAllDevicesOutputDTO> { new() { DeviceId = Guid.NewGuid().ToString(), Status = "Active" } },
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 1
        };
        _mockApp.Setup(x => x.GetAllDevices(pagination)).ReturnsAsync(result);

        // Act
        var response = await _controller.GetAllDevices(pagination);

        // Assert
        var okResult = response.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task OccupySpot_ShouldReturnNoContent()
    {
        var deviceId = Guid.NewGuid();
        _mockApp.Setup(x => x.Occupy(deviceId)).Returns(Task.CompletedTask);

        var result = await _controller.OccupySpot(deviceId);

        result.Should().BeOfType<StatusCodeResult>()
              .Which.StatusCode.Should().Be((int)Enums.StatusCode.NoContent);
    }

    [Fact]
    public async Task OccupySpot_ShouldThrowRateLimitExceeded()
    {
        var deviceId = Guid.NewGuid();
        var exception = new BadResponseWithMessage("Rate limit exceeded", (int)Enums.StatusCode.TooManyRequests);

        _mockApp.Setup(x => x.Occupy(deviceId)).ThrowsAsync(exception);

        var result = await Assert.ThrowsAsync<BadResponseWithMessage>(() => _controller.OccupySpot(deviceId));

        result.StatusCode.Should().Be((int)Enums.StatusCode.TooManyRequests);
        result.Message.Should().Be("Rate limit exceeded");
    }

    [Fact]
    public async Task OccupySpot_ShouldThrowAlreadyOccupied()
    {
        var deviceId = Guid.NewGuid();
        var exception = new BadResponseWithMessage("Parking spot already occupied", (int)Enums.StatusCode.Conflict);

        _mockApp.Setup(x => x.Occupy(deviceId)).ThrowsAsync(exception);

        var result = await Assert.ThrowsAsync<BadResponseWithMessage>(() => _controller.OccupySpot(deviceId));

        result.StatusCode.Should().Be((int)Enums.StatusCode.Conflict);
        result.Message.Should().Be("Parking spot already occupied");
    }

    [Fact]
    public async Task FreeSpot_ShouldReturnNoContent()
    {
        var deviceId = Guid.NewGuid();
        _mockApp.Setup(x => x.Free(deviceId)).Returns(Task.CompletedTask);

        var result = await _controller.FreeSpot(deviceId);

        result.Should().BeOfType<StatusCodeResult>()
              .Which.StatusCode.Should().Be((int)Enums.StatusCode.NoContent);
    }

    [Fact]
    public async Task FreeSpot_ShouldThrowDeviceNotFound()
    {
        var deviceId = Guid.NewGuid();
        var ex = new BadResponseWithMessage("Invalid device", (int)Enums.StatusCode.NotFound);
        _mockApp.Setup(x => x.Free(deviceId)).ThrowsAsync(ex);

        var result = await Assert.ThrowsAsync<BadResponseWithMessage>(() => _controller.FreeSpot(deviceId));

        result.StatusCode.Should().Be((int)Enums.StatusCode.NotFound);
        result.Message.Should().Be("Invalid device");
    }

    [Fact]
    public async Task GetParkingSpotsStatus_ShouldReturnOk()
    {
        var pagination = new PaginationParams { PageNumber = 1, PageSize = 10 };
        var result = new PagedResult<GetSpotsOutputDTO>
        {
            Items = new List<GetSpotsOutputDTO>
            {
                new() { ParkingSpotId = Guid.NewGuid().ToString(), ParkingName = "A1", Status = "Free" }
            },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _mockApp.Setup(x => x.GetParkingSpotsStatus(pagination)).ReturnsAsync(result);

        var response = await _controller.GetParkingSpotsStatus(pagination);

        var okResult = response.Should().BeOfType<ObjectResult>().Subject;
        okResult.StatusCode.Should().Be((int)Enums.StatusCode.Ok);
        okResult.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task CreateSpot_ShouldReturnCreated()
    {
        var dto = new CreateSpotInputDTO
        {
            DeviceId = Guid.NewGuid(),
            ParkingSpotId = Guid.NewGuid()
        };

        var createdDto = new CreateSpotOutputDTO
        {
            DeviceId = dto.DeviceId.ToString(),
            ParkingSpotId = dto.ParkingSpotId.ToString(),
            Message = "Created"
        };

        _mockApp.Setup(x => x.CreateParkingSpot(dto)).ReturnsAsync(createdDto);

        var result = await _controller.CreateSpot(dto);

        var createdResult = result.Should().BeOfType<ObjectResult>().Subject;
        createdResult.StatusCode.Should().Be((int)Enums.StatusCode.Created);
        createdResult.Value.Should().BeEquivalentTo(createdDto);
    }

    [Fact]
    public async Task CreateSpot_ShouldThrowConflict()
    {
        var dto = new CreateSpotInputDTO
        {
            DeviceId = Guid.NewGuid(),
            ParkingSpotId = Guid.NewGuid()
        };

        var ex = new BadResponseWithMessage("Spot already linked", (int)Enums.StatusCode.Conflict);
        _mockApp.Setup(x => x.CreateParkingSpot(dto)).ThrowsAsync(ex);

        var result = await Assert.ThrowsAsync<BadResponseWithMessage>(() => _controller.CreateSpot(dto));

        result.StatusCode.Should().Be((int)Enums.StatusCode.Conflict);
        result.Message.Should().Be("Spot already linked");
    }

    [Fact]
    public async Task DeleteSpot_ShouldReturnNoContent()
    {
        var spotId = Guid.NewGuid();
        _mockApp.Setup(x => x.DeleteSpot(spotId)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteSpot(spotId);

        result.Should().BeOfType<StatusCodeResult>()
              .Which.StatusCode.Should().Be((int)Enums.StatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteSpot_ShouldThrowNotFound()
    {
        var spotId = Guid.NewGuid();
        var ex = new BadResponseWithMessage("Spot not found", (int)Enums.StatusCode.NotFound);
        _mockApp.Setup(x => x.DeleteSpot(spotId)).ThrowsAsync(ex);

        var result = await Assert.ThrowsAsync<BadResponseWithMessage>(() => _controller.DeleteSpot(spotId));

        result.StatusCode.Should().Be((int)Enums.StatusCode.NotFound);
        result.Message.Should().Be("Spot not found");
    }
}
