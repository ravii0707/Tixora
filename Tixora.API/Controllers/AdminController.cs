using Microsoft.AspNetCore.Mvc;
using Tixora.API.Middleware;

namespace Tixora.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpGet("apistats")]
        public IActionResult GetApiStats()
        {
            var stats = RequestLoggingMiddleware.GetSuccessRate();
            return Ok(stats);
        }
    }
}

