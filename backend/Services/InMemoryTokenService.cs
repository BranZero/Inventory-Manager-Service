using System.Collections.Concurrent;

public class InMemoryTokenRevocationService : ITokenRevocationService
{
    // jti -> expiry
    private readonly ConcurrentDictionary<string, DateTime> _revoked = new();

    public void RevokeToken(string jti, DateTime expiresAt)
    {
        if (string.IsNullOrEmpty(jti)) return;
        _revoked[jti] = expiresAt;
    }

    public bool IsRevoked(string jti)
    {
        if (string.IsNullOrEmpty(jti)) return false;
        if (!_revoked.TryGetValue(jti, out var exp)) return false;

        // remove expired entries lazily
        if (exp < DateTime.UtcNow)
        {
            _revoked.TryRemove(jti, out _);
            return false;
        }

        return true;
    }
}