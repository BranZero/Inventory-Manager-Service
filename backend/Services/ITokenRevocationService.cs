public interface ITokenRevocationService
{
    void RevokeToken(string jti, DateTime expiresAt);
    bool IsRevoked(string jti);
}