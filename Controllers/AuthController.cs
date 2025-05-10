using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TSBackend.Model;
using TSBackend.Data;
using DotNetEnv;

namespace TSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponseDto>> Register(UserCreateDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return Conflict(new { message = "Email уже зарегистрирован" });


            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                UserName = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return Ok(new LoginResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Token = token
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(UserLoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Неверный email или пароль" });

            var token = GenerateJwtToken(user);

            return Ok(new LoginResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email ?? string.Empty,
                Token = token
            });
        }


        // Генерация JWT токена
        private string GenerateJwtToken(User user)
        {
            var jwtSecret = Env.GetString("JWT_SECRET") ?? "YourSuperSecretKey123!@#$ThisShouldBeAtLeast32BytesLong";
            var jwtIssuer = Env.GetString("JWT_ISSUER") ?? "YourIssuer";
            var jwtAudience = Env.GetString("JWT_AUDIENCE") ?? "YourAudience";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? "UnknownUser"),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTO для ответа при логине
    public class LoginResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}