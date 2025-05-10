// HallsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TSBackend.Model;
using TSBackend.Data;

namespace TSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HallsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HallsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hall>>> GetHalls()
        {
            return await _context.Halls
                .Include(h => h.Venue)
                .Include(h => h.Rows)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Hall>> GetHall(int id)
        {
            var hall = await _context.Halls
                .Include(h => h.Venue)
                .Include(h => h.Rows)
                .ThenInclude(r => r.Seats)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hall == null)
            {
                return NotFound();
            }

            return hall;
        }

        [HttpPost]
        public async Task<ActionResult<Hall>> PostHall(HallCreateDto hallDto)
        {
            var venueExists = await _context.Venues.AnyAsync(v => v.Id == hallDto.VenueId);
            if (!venueExists)
            {
                return BadRequest("Venue does not exist");
            }

            // Проверка на уникальность имени зала в месте проведения
            if (await _context.Halls.AnyAsync(h => h.VenueId == hallDto.VenueId && h.Name == hallDto.Name))
            {
                return Conflict("Hall with this name already exists in the venue");
            }

            var hall = new Hall
            {
                Name = hallDto.Name,
                Capacity = hallDto.Capacity,
                VenueId = hallDto.VenueId
            };

            _context.Halls.Add(hall);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHall), new { id = hall.Id }, hall);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutHall(int id, HallUpdateDto hallDto)
        {
            var hall = await _context.Halls.FindAsync(id);
            if (hall == null)
            {
                return NotFound();
            }

            // Проверка на уникальность имени зала в месте проведения при обновлении
            if (hallDto.Name != hall.Name &&
                await _context.Halls.AnyAsync(h => h.VenueId == hall.VenueId && h.Name == hallDto.Name && h.Id != id))
            {
                return Conflict("Hall with this name already exists in the venue");
            }

            hall.Name = hallDto.Name;
            hall.Capacity = hallDto.Capacity;

            _context.Entry(hall).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HallExists(id))
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
        public async Task<IActionResult> DeleteHall(int id)
        {
            var hall = await _context.Halls.FindAsync(id);
            if (hall == null)
            {
                return NotFound();
            }

            // Проверка наличия мероприятий перед удалением
            if (await _context.Meetings.AnyAsync(m => m.HallId == id))
            {
                return BadRequest("Cannot delete hall with existing meetings");
            }

            // Проверка наличия рядов перед удалением
            if (await _context.Rows.AnyAsync(r => r.HallId == id))
            {
                return BadRequest("Cannot delete hall with existing rows");
            }

            _context.Halls.Remove(hall);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HallExists(int id)
        {
            return _context.Halls.Any(e => e.Id == id);
        }
    }
}