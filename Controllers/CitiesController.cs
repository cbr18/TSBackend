// CitiesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TSBackend.Model;
using TSBackend.Data;

namespace TSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            return await _context.Cities.Include(c => c.Venues).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            var city = await _context.Cities.Include(c => c.Venues).FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
            {
                return NotFound();
            }

            return city;
        }

        [HttpPost]
        public async Task<ActionResult<City>> PostCity(CityCreateDto cityDto)
        {
            if (_context.Cities.Any(c => c.Name == cityDto.Name))
            {
                return Conflict("City with this name already exists");
            }

            var city = new City
            {
                Name = cityDto.Name
            };

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { id = city.Id }, city);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, City city)
        {
            if (id != city.Id)
            {
                return BadRequest();
            }

            _context.Entry(city).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            if (await _context.Venues.AnyAsync(v => v.CityId == id))
            {
                return BadRequest("Cannot delete city with associated venues");
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}
