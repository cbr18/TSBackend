// VenuesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TSBackend.Model;
using TSBackend.Data;

namespace TSBackend.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase {
        private readonly AppDbContext _context;

        public VenuesController(AppDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venue>>> GetVenues() {
            return await _context.Venues
                .Include(v => v.City)
                .Include(v => v.Halls)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Venue>> GetVenue(int id) {
            var venue = await _context.Venues
                .Include(v => v.City)
                .Include(v => v.Halls)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venue == null) {
                return NotFound();
            }

            return venue;
        }

        [HttpPost]
        public async Task<ActionResult<Venue>> PostVenue(VenueCreateDto venueDto) {
            var cityExists = await _context.Cities.AnyAsync(c => c.Id == venueDto.CityId);
            if (!cityExists) {
                return BadRequest("City does not exist");
            }

            // Проверка на уникальность адреса в городе
            if (await _context.Venues.AnyAsync(v => v.CityId == venueDto.CityId && v.Address == venueDto.Address)) {
                return Conflict("Venue with this address already exists in the city");
            }

            var venue = new Venue {
                Name = venueDto.Name,
                Address = venueDto.Address,
                CityId = venueDto.CityId
            };

            _context.Venues.Add(venue);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVenue), new { id = venue.Id }, venue);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVenue(int id, VenueUpdateDto venueDto) {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) {
                return NotFound();
            }

            // Проверка на уникальность адреса в городе при обновлении
            if (venueDto.Address != venue.Address &&
                await _context.Venues.AnyAsync(v => v.CityId == venue.CityId && v.Address == venueDto.Address && v.Id != id)) {
                return Conflict("Venue with this address already exists in the city");
            }

            venue.Name = venueDto.Name; // Ensure Name is set during update
            venue.Address = venueDto.Address;
            _context.Entry(venue).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!VenueExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenue(int id) {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) {
                return NotFound();
            }

            // Проверка наличия залов перед удалением
            if (await _context.Halls.AnyAsync(h => h.VenueId == id)) {
                return BadRequest("Cannot delete venue with existing halls");
            }

            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VenueExists(int id) {
            return _context.Venues.Any(e => e.Id == id);
        }
    }
}