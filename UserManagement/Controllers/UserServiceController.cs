using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using static UserManagement.UserRequest;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserServiceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserServiceController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = new List<string> { "Jan", "User2" };
            return Ok(users);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Users.AnyAsync(u => u.Id == request.Id))
            {
                return Conflict("User already exists.");
            }

            var user = new User
            {
                Id = request.Id,
                Username = request.Username,
                Bio = request.Bio,
                Auth0Id = request.Id.ToString(),
                DisplayName = request.Username,
                Email = string.Empty,
                ProfilePictureUrl = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.Username, user.Bio });
        }
    }
}
