using Microsoft.AspNetCore.Mvc;

namespace TestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Home()
        {
            return Ok(new { message = "TestAPI is running" });
        }

        [HttpGet("hello-world")]
        public IActionResult HelloWorld()
        {
            return Ok(new { message = "Hello AWS community, enjoy Community Day at UPB!" });
        }
    }
}
