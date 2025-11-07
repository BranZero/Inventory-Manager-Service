using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService
{
    private readonly IConfiguration _cfg;
    public TokenService(IConfiguration cfg) => _cfg = cfg;
    public string CreateToken(string username)
    {
        var key = Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!);
        var issuer = _cfg["Jwt:Issuer"];
        var claims = new[] { new Claim(ClaimTypes.Name, username) };
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer, null, claims, expires: DateTime.UtcNow.AddHours(6), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}