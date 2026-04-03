// Controllers/ManufacturersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestProject.Dbo;

[ApiController]
[Route("api/[controller]")]
public class ManufacturersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public ManufacturersController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET: api/manufacturers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ManufacturerDBO>>> GetManufacturers()
    {
        return await _context.Manufacturers
            //.Include(m => m.CPUs)
            .ToListAsync();
    }
    
    // GET: api/manufacturers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ManufacturerDBO>> GetManufacturer(int id)
    {
        var manufacturer = await _context.Manufacturers
            //.Include(m => m.CPUs)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (manufacturer == null)
            return NotFound();
        
        return manufacturer;
    }
    
    // POST: api/manufacturers
    [HttpPost]
    public async Task<ActionResult<ManufacturerDBO>> CreateManufacturer(ManufacturerDBO manufacturer)
    {
        _context.Manufacturers.Add(manufacturer);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetManufacturer), new { id = manufacturer.Id }, manufacturer);
    }
}