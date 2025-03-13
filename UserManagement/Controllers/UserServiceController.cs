using Microsoft.AspNetCore.Mvc;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserServiceController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = new List<string> { "User1", "User2" };
            return Ok(users);
        }
    }
}