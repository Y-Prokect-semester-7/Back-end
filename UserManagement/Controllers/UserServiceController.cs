using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserServiceController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = new List<string> { "Jan", "User2" };
            return Ok(users);
        }
    }
}