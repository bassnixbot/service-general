public class CooldownService
{
    private readonly Dictionary<string, DateTime> _lastRequestTimes = new Dictionary<string, DateTime>();
    private readonly TimeSpan _cooldownDuration = TimeSpan.FromSeconds(10);

    public bool IsCooldownActive(string key)
    {
        if (_lastRequestTimes.TryGetValue(key, out var lastRequestTime))
        {
            // Check if the cooldown period has expired
            if (DateTime.UtcNow - lastRequestTime < _cooldownDuration)
            {
                return true; // Cooldown is still active
            }
        }

        // Update the last request time
        _lastRequestTimes[key] = DateTime.UtcNow;
        return false; // Cooldown has expired, or this is the first request
    }
}
