using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class WarehousesController : ControllerBase
{
    private readonly AppDbContext _db;
    public WarehousesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Warehouses.ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var warehouse = await _db.Warehouses
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.Id == id);
        
        if (warehouse == null) return NotFound();
        return Ok(warehouse);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(WarehouseCreateDto dto)
    {
        var warehouse = new Warehouse { Name = dto.Name, Location = dto.Location, CompanyId = dto.CompanyId };
        _db.Warehouses.Add(warehouse);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = warehouse.Id }, warehouse);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, WarehouseUpdateDto dto)
    {
        var warehouse = await _db.Warehouses.FindAsync(id);
        if (warehouse == null) return NotFound();
        
        warehouse.Name = dto.Name;
        warehouse.Location = dto.Location;
        await _db.SaveChangesAsync();
        return Ok(warehouse);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var warehouse = await _db.Warehouses.FindAsync(id);
        if (warehouse == null) return NotFound();
        
        _db.Warehouses.Remove(warehouse);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record WarehouseCreateDto(string Name, string Location, int CompanyId);
public record WarehouseUpdateDto(string Name, string Location);
