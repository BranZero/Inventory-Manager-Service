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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _db.Items.FirstOrDefaultAsync(i => i.Id == id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpGet("warehouse/{warehouseId}")]
    public async Task<IActionResult> GetByWarehouse(int warehouseId) 
        => Ok(await _db.Items.Where(i => i.WarehouseId == warehouseId).ToListAsync());

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(ItemCreateDto dto)
    {
        var item = new Item 
        { 
            Sku = dto.Sku, 
            Name = dto.Name, 
            Description = dto.Description,
            Quantity = dto.Quantity,
            WarehouseId = dto.WarehouseId
        };
        _db.Items.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ItemUpdateDto dto)
    {
        var item = await _db.Items.FindAsync(id);
        if (item == null) return NotFound();
        
        item.Name = dto.Name;
        item.Description = dto.Description;
        item.Quantity = dto.Quantity;
        item.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(item);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Items.FindAsync(id);
        if (item == null) return NotFound();
        
        _db.Items.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record ItemCreateDto(string Sku, string Name, string Description, int Quantity, int WarehouseId);
public record ItemUpdateDto(string Name, string Description, int Quantity);
