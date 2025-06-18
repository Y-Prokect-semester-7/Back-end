using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using static UserManagement.UserRequest;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/adduser")]
    public class UserServiceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserServiceController(AppDbContext context)
        {
            _context = context;
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Users.AnyAsync(u => u.Auth0Id == request.Auth0Id))
            {
                return Conflict("User already exists.");
            }

            var user = new User
            {
                Auth0Id = request.Auth0Id,
                Username = request.Username,
                Bio = request.Bio,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new {  user.Username, user.Bio });
        }
    }
}
