namespace LaWash.IoT.Domain;
public interface IRateLimiter
{
    bool IsRequestAllowed(Guid deviceId);
}
