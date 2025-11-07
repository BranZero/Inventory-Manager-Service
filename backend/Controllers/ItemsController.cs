using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ItemsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Items.ToListAsync());

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(ItemCreateDto dto)
    {
        var item = new Item { Sku = dto.Sku, Name = dto.Name, Quantity = dto.Quantity };
        _db.Items.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = item.Id }, item);
    }
}

public record ItemCreateDto(string Sku, string Name, int Quantity);