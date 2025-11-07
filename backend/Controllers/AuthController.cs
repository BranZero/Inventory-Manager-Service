using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokens;
    public AuthController(AppDbContext db, TokenService tokens) { _db = db; _tokens = tokens; }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Username == dto.Username)) return BadRequest("User exists");
        CreatePasswordHash(dto.Password, out var hash, out var salt);
        var user = new User { Username = dto.Username, PasswordHash = hash, PasswordSalt = salt };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null) return Unauthorized();

        if (!VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt)) return Unauthorized();
        var token = _tokens.CreateToken(user.Username);
        return Ok(new { token });
    }

    static void CreatePasswordHash(string pwd, out byte[] hash, out byte[] salt)
    {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pwd));
    }
    static bool VerifyPasswordHash(string pwd, byte[] hash, byte[] salt)
    {
        using var hmac = new HMACSHA512(salt);
        var comp = hmac.ComputeHash(Encoding.UTF8.GetBytes(pwd));
        return comp.SequenceEqual(hash);
    }
}

public record RegisterDto(string Username, string Password);
public record LoginDto(string Username, string Password);