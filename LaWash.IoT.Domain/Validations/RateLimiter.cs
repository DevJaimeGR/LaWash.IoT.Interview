namespace LaWash.IoT.Domain;
public class RateLimiter : IRateLimiter
{
    private readonly Dictionary<Guid,DateTime> _lastRequestTimes = new Dictionary<Guid, DateTime>();
    private readonly TimeSpan _timeLimit = TimeSpan.FromSeconds(10);
    private readonly TimeSpan _timeToClear = TimeSpan.FromMinutes(10);

    public bool IsRequestAllowed(Guid deviceId)
    {
        var now = DateTime.UtcNow;

        var keysToRemove = _lastRequestTimes
            .Where(kvp => now - kvp.Value > _timeToClear)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
            _lastRequestTimes.Remove(key);

        if (_lastRequestTimes.TryGetValue(deviceId, out var lastTime))
        {
            if (now - lastTime < _timeLimit)
                return false;
        }

        _lastRequestTimes[deviceId] = now;
        return true;
    }
}

