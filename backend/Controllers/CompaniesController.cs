using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly AppDbContext _db;
    public CompaniesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Companies.ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var company = await _db.Companies
            .Include(c => c.Users)
            .Include(c => c.Warehouses)
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (company == null) return NotFound();
        return Ok(company);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CompanyCreateDto dto)
    {
        var company = new Company { Name = dto.Name };
        _db.Companies.Add(company);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = company.Id }, company);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CompanyUpdateDto dto)
    {
        var company = await _db.Companies.FindAsync(id);
        if (company == null) return NotFound();
        
        company.Name = dto.Name;
        await _db.SaveChangesAsync();
        return Ok(company);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var company = await _db.Companies.FindAsync(id);
        if (company == null) return NotFound();
        
        _db.Companies.Remove(company);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record CompanyCreateDto(string Name);
public record CompanyUpdateDto(string Name);
