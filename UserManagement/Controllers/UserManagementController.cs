using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;


namespace UserManagement.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserManagementController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserManagementController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("exists/{auth0Id}")]
        public async Task<IActionResult> UserExists(string auth0Id)
        {
            var exists = await _context.Users.AnyAsync(u => u.Auth0Id == auth0Id);
            return Ok(new { exists });
        }
    }
}
