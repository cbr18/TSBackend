// RowsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TSBackend.Model;
using TSBackend.Data;

namespace TSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RowsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RowsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Rows
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Row>>> GetRows()
        {
            return await _context.Rows
                .Include(r => r.Hall)
                .Include(r => r.Seats)
                .ToListAsync();
        }

        // GET: api/Rows/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Row>> GetRow(int id)
        {
            var row = await _context.Rows
                .Include(r => r.Hall)
                .Include(r => r.Seats)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (row == null)
            {
                return NotFound();
            }

            return row;
        }

        // GET: api/Rows/hall/5
        [HttpGet("hall/{hallId}")]
        public async Task<ActionResult<IEnumerable<Row>>> GetRowsByHall(int hallId)
        {
            var hallExists = await _context.Halls.AnyAsync(h => h.Id == hallId);
            if (!hallExists)
            {
                return NotFound("Hall not found");
            }

            return await _context.Rows
                .Where(r => r.HallId == hallId)
                .Include(r => r.Seats)
                .OrderBy(r => r.RowNumber)
                .ToListAsync();
        }

        // POST: api/Rows
        [HttpPost]
        public async Task<ActionResult<Row>> PostRow(RowCreateDto rowDto)
        {
            var hallExists = await _context.Halls.AnyAsync(h => h.Id == rowDto.HallId);
            if (!hallExists)
            {
                return BadRequest("Hall does not exist");
            }

            if (await _context.Rows.AnyAsync(r => r.HallId == rowDto.HallId && r.RowNumber == rowDto.RowNumber))
            {
                return BadRequest("Row with this number already exists in the specified hall");
            }

            var row = new Row
            {
                RowNumber = rowDto.RowNumber,
                SeatsCount = rowDto.SeatsCount,
                HallId = rowDto.HallId
            };

            _context.Rows.Add(row);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRow), new { id = row.Id }, row);
        }

        // PUT: api/Rows/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRow(int id, RowUpdateDto rowDto)
        {
            if (id != rowDto.Id)
            {
                return BadRequest("ID in URL does not match ID in body");
            }

            var row = await _context.Rows.FindAsync(id);
            if (row == null)
            {
                return NotFound();
            }

            if (rowDto.RowNumber != row.RowNumber &&
                await _context.Rows.AnyAsync(r => r.HallId == row.HallId && r.RowNumber == rowDto.RowNumber && r.Id != id))
            {
                return BadRequest("Row with this number already exists in the hall");
            }

            row.RowNumber = rowDto.RowNumber;
            row.SeatsCount = rowDto.SeatsCount;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RowExists(id))
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

        // DELETE: api/Rows/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRow(int id)
        {
            var row = await _context.Rows
                .Include(r => r.Seats)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (row == null)
            {
                return NotFound();
            }

            if (row.Seats.Any())
            {
                return BadRequest("Cannot delete row because it has associated seats");
            }

            _context.Rows.Remove(row);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RowExists(int id)
        {
            return _context.Rows.Any(e => e.Id == id);
        }
    }
}