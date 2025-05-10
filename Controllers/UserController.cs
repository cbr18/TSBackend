using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TSBackend.Model;
using TSBackend.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Защищаем все методы по умолчанию
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // Получение текущего пользователя
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (user == null)
                return NotFound();

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email ?? string.Empty
            };
        }

        // Получение профиля пользователя
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null || int.Parse(currentUserId) != id)
                return Forbid();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email ?? string.Empty
            };
        }

        // Получение списка всех пользователей (только для администраторов)
        [HttpGet]
        [Authorize] // Пример ограничения доступа ролью
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email ?? string.Empty
            }).ToList();
        }

        // Обновление данных пользователя
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto dto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null || int.Parse(currentUserId) != id)
                return Forbid();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            // Обновляем только если новое значение предоставлено
            user.Name = !string.IsNullOrWhiteSpace(dto.Name) ? dto.Name : user.Name;
            user.Email = !string.IsNullOrWhiteSpace(dto.Email) ? dto.Email : user.Email;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Получение билетов пользователя
        [HttpGet("{id}/tickets")]
        public async Task<ActionResult<IEnumerable<TicketDetailsDto>>> GetUserTickets(int id)
        {
            var user = await _context.Users
                .Include(u => u.Tickets)
                .ThenInclude(t => t.Meeting)
                .ThenInclude(m => m.Hall)
                .ThenInclude(h => h.Venue)
                .Include(u => u.Tickets)
                .ThenInclude(t => t.Seat)
                .ThenInclude(s => s.Row)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            var tickets = user.Tickets.Select(t => new TicketDetailsDto
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
                UserName = user.Name,
                PurchaseDate = t.PurchaseDate
            });

            return Ok(tickets);
        }

        // Удаление пользователя
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null || int.Parse(currentUserId) != id)
                return Forbid();

            var user = await _context.Users
                .Include(u => u.Tickets)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            if (user.Tickets.Any())
                return BadRequest("Невозможно удалить пользователя с существующими билетами");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // DTO для представления пользователя
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    // DTO для обновления пользователя
    public class UserUpdateDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}