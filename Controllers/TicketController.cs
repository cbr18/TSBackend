// TicketController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TSBackend.Data;
using TSBackend.Model;

namespace TSBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketController : ControllerBase
{
    private readonly AppDbContext _context;

    public TicketController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketDetailsDto>>> GetTickets()
    {
        var tickets = await _context.Tickets
            .Include(t => t.Meeting)
            .ThenInclude(m => m.Hall)
            .ThenInclude(h => h.Venue)
            .Include(t => t.User)
            .Include(t => t.Seat)
            .ThenInclude(s => s.Row)
            .Select(t => new TicketDetailsDto
            {
                Id = t.Id,
                MeetingName = t.Meeting.Name,
                MeetingDate = t.Meeting.Date,
                MeetingId = t.Meeting.Id,
                UserId = t.User.Id,
                VenueName = t.Meeting.Hall.Venue.Address,
                HallName = t.Meeting.Hall.Name,
                RowNumber = t.Seat.Row.RowNumber,
                SeatNumber = t.Seat.SeatNumber,
                UserName = t.User.Name,
                PurchaseDate = t.PurchaseDate
            })
            .ToListAsync();

        return tickets;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDetailsDto>> GetTicket(int id)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Meeting)
            .ThenInclude(m => m.Hall)
            .ThenInclude(h => h.Venue)
            .Include(t => t.User)
            .Include(t => t.Seat)
            .ThenInclude(s => s.Row)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
            return NotFound();

        var ticketDto = new TicketDetailsDto
        {
            Id = ticket.Id,
            MeetingName = ticket.Meeting.Name,
            MeetingDate = ticket.Meeting.Date,
            MeetingId = ticket.Meeting.Id,
            UserId = ticket.User.Id,
            VenueName = ticket.Meeting.Hall.Venue.Address,
            HallName = ticket.Meeting.Hall.Name,
            RowNumber = ticket.Seat.Row.RowNumber,
            SeatNumber = ticket.Seat.SeatNumber,
            UserName = ticket.User.Name,
            PurchaseDate = ticket.PurchaseDate
        };

        return ticketDto;
    }

    [HttpPost]
    public async Task<ActionResult<Ticket>> Create(TicketCreateDto ticketDto)
    {
        var meeting = await _context.Meetings
            .Include(m => m.Hall)
            .FirstOrDefaultAsync(m => m.Id == ticketDto.MeetingId);

        if (meeting == null)
            return BadRequest("Meeting not found");

        if (!await _context.Users.AnyAsync(u => u.Id == ticketDto.UserId))
            return BadRequest("User not found");

        var seat = await _context.Seats
            .Include(s => s.Row)
            .ThenInclude(r => r.Hall)
            .FirstOrDefaultAsync(s => s.Id == ticketDto.SeatId);

        if (seat == null)
            return BadRequest("Seat not found");

        // Проверка, что место принадлежит залу мероприятия
        if (seat.Row.HallId != meeting.HallId)
            return BadRequest("Seat does not belong to meeting hall");

        // Проверка, что место не занято на этом мероприятии
        if (await _context.Tickets.AnyAsync(t => t.MeetingId == ticketDto.MeetingId && t.SeatId == ticketDto.SeatId))
            return BadRequest("Seat is already booked for this meeting");

        var ticket = new Ticket
        {
            MeetingId = ticketDto.MeetingId,
            UserId = ticketDto.UserId,
            SeatId = ticketDto.SeatId
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
            return NotFound();

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}