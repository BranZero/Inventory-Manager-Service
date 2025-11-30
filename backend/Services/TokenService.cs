using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService
{
    private readonly IConfiguration _cfg;
    public TokenService(IConfiguration cfg) => _cfg = cfg;
    public (string token, string jti, DateTime expires) CreateToken(string username)
    {
        var key = Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!);
        var issuer = _cfg["Jwt:Issuer"];
        var expires = DateTime.UtcNow.AddHours(6);

        var jti = Guid.NewGuid().ToString();

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, jti)
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer, null, claims, expires: expires, signingCredentials: creds);
        var written = new JwtSecurityTokenHandler().WriteToken(token);
        return (written, jti, expires);
    }
}