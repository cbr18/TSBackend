// MeetingsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TSBackend.Model;
using TSBackend.Data;

namespace TSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MeetingsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Meeting>>> GetMeetings()
        {
            return await _context.Meetings
                .Include(m => m.Tickets)
                .Include(m => m.Hall)
                .ThenInclude(h => h.Venue)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Meeting>> GetMeeting(int id)
        {
            var meeting = await _context.Meetings
                .Include(m => m.Hall)
                .ThenInclude(h => h.Venue)
                .Include(m => m.Tickets)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meeting == null)
            {
                return NotFound();
            }

            return meeting;
        }

        [HttpPost]
        public async Task<ActionResult<Meeting>> PostMeeting(MeetingCreateDto meetingDto)
        {
            var hallExists = await _context.Halls.AnyAsync(h => h.Id == meetingDto.HallId);
            if (!hallExists)
            {
                return BadRequest("Hall does not exist");
            }

            var meeting = new Meeting
            {
                Name = meetingDto.Name,
                Date = meetingDto.Date,
                HallId = meetingDto.HallId,
                Description = meetingDto.Description,
                HasSeatMap = meetingDto.HasSeatMap
            };

            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMeeting", new { id = meeting.Id }, meeting);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeeting(int id, MeetingUpdateDto meetingDto)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return NotFound();
            }

            meeting.Name = meetingDto.Name ?? meeting.Name;
            meeting.Description = meetingDto.Description ?? meeting.Description;
            meeting.Date = meetingDto.Date;
            meeting.HasSeatMap = meetingDto.HasSeatMap;

            _context.Entry(meeting).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeetingExists(id))
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
        public async Task<IActionResult> DeleteMeeting(int id)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return NotFound();
            }

            // Проверка наличия билетов перед удалением
            if (await _context.Tickets.AnyAsync(t => t.MeetingId == id))
            {
                return BadRequest("Cannot delete meeting with existing tickets");
            }

            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MeetingExists(int id)
        {
            return _context.Meetings.Any(e => e.Id == id);
        }
    }
}