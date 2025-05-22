using LaWash.IoT.Application;
using LaWash.IoT.Domain;
using LaWash.IoT.Infraestructure;
using LaWash.IoT.Transversal;

namespace LaWash.Iot.API;

/// <summary>
/// Provides extension methods for registering custom services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds custom services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IParkingApplication, ParkingApplication>();
        services.AddSingleton<IRateLimiter, RateLimiter>();
        services.AddScoped<ICreateParkingSpotStrategy, SpotAndDeviceDoNotExistStrategy>();
        services.AddScoped<ICreateParkingSpotStrategy, SpotExistsAndDeviceDoesNotExistStrategy>();
        services.AddScoped<ICreateParkingSpotStrategy, SpotDoesNotExistAndDeviceExistsStrategy>();
        services.AddScoped<ICreateParkingSpotStrategy, SpotAndDeviceExistsStrategy>();
        services.AddTransient<ExceptionMiddleware>();
        services.AddScoped<InitializerData>();

        return services;
    }
}

