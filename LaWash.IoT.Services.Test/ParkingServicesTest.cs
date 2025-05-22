using Moq;
using FluentAssertions;
using LaWash.IoT.Transversal;
using LaWash.IoT.Domain;
using LaWash.IoT.Infraestructure;
using System.Linq.Expressions;

namespace LaWash.IoT.Application.Tests;

public class ParkingApplicationTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IRateLimiter> _rateLimiter;
    private readonly ParkingApplication _service;

    public ParkingApplicationTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _rateLimiter = new Mock<IRateLimiter>();
        _service = new ParkingApplication(_unitOfWork.Object, _rateLimiter.Object);
    }

    [Fact]
    public async Task Occupy_ShouldSucceed_WhenDeviceAndSpotExistAndIsFree()
    {
        var deviceId = Guid.NewGuid();

        _rateLimiter.Setup(x => x.IsRequestAllowed(deviceId)).Returns(true);

        _unitOfWork.Setup(x => x.Devices.FindAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .ReturnsAsync(new Device { Id = deviceId.ToString(), IsDeleted = false });

        _unitOfWork.Setup(x => x.ParkingSpotsDevices.FindAsync(It.IsAny<Expression<Func<ParkingSpotsDevice, bool>>>()))
            .ReturnsAsync(new ParkingSpotsDevice { DeviceId = deviceId.ToString(), ParkingSpotId = Guid.NewGuid().ToString(), IsDeleted = false });

        _unitOfWork.Setup(x => x.ParkingSpotsStatus.FindAsync(It.IsAny<Expression<Func<ParkingSpotsStatus, bool>>>()))
           .ReturnsAsync(new ParkingSpotsStatus { ParkingSpotId = Guid.NewGuid().ToString(), Status = false });


        _unitOfWork.Setup(x => x.ParkingSpotsStatus.UpdateAsync(It.IsAny<ParkingSpotsStatus>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.Occupy(deviceId);
    }

    [Fact]
    public async Task Occupy_ShouldThrow_WhenRateLimitExceeded()
    {
        var deviceId = Guid.NewGuid();
        _rateLimiter.Setup(x => x.IsRequestAllowed(deviceId)).Returns(false);

        var act = async () => await _service.Occupy(deviceId);

        var ex = await Assert.ThrowsAsync<BadResponseWithMessage>(act);
        ex.StatusCode.Should().Be((int)Enums.StatusCode.TooManyRequests);
    }

    [Fact]
    public async Task Free_ShouldThrow_WhenDeviceNotFound()
    {
        var deviceId = Guid.NewGuid();

        _rateLimiter.Setup(x => x.IsRequestAllowed(deviceId)).Returns(true);
        _unitOfWork.Setup(x => x.Devices.FindAsync(It.IsAny<Expression<Func<Device, bool>>>()))
            .ReturnsAsync((Device?)null); // Fix: Explicitly cast null to nullable type  

        var act = async () => await _service.Free(deviceId);

        var ex = await Assert.ThrowsAsync<BadResponseWithMessage>(act);
        ex.StatusCode.Should().Be((int)Enums.StatusCode.NotFound);
        ex.Message.Should().Be("Invalid device");
    }

    [Fact]
    public async Task GetAllDevices_ShouldReturnRegisteredAndUnregisteredStatuses()
    {
        var pagination = new PaginationParams { PageNumber = 1, PageSize = 10 };

        _unitOfWork.Setup(x => x.Devices.GetPagedAsync(It.IsAny<Expression<Func<Device, bool>>>(), pagination))
            .ReturnsAsync(new PagedResult<Device>
            {
                Items = new List<Device>
                {
                new() { Id = "1", IsDeleted = false, IsWorking = true },
                new() { Id = "2", IsDeleted = false, IsWorking = true }
                },
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 10
            });

        _unitOfWork.Setup(x => x.ParkingSpotsDevices.FindAsync(It.IsAny<Expression<Func<ParkingSpotsDevice, bool>>>()))
            .Returns<Expression<Func<ParkingSpotsDevice, bool>>>(expr =>
            {
                var predicate = expr.Compile();

                var device1 = new ParkingSpotsDevice { DeviceId = "1", ParkingSpotId = Guid.NewGuid().ToString() };
                var device2 = new ParkingSpotsDevice { DeviceId = "2", ParkingSpotId = Guid.NewGuid().ToString() };

                if (predicate(device1))
                    return Task.FromResult<ParkingSpotsDevice?>(device1);

                if (predicate(device2))
                    return Task.FromResult<ParkingSpotsDevice?>(null);

                return Task.FromResult<ParkingSpotsDevice?>(null); 
            });

        var result = await _service.GetAllDevices(pagination);

        result.Items.Should().HaveCount(2);
        result.Items.Should().Contain(x => x.DeviceId.ToString() == "1" && x.Status == "Registered and in use");
        result.Items.Should().Contain(x => x.DeviceId.ToString() == "2" && x.Status == "Registered but not in use");
    }


}
