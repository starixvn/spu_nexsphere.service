using Microsoft.AspNetCore.Mvc;

namespace External.CTS.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        [HttpGet("users")]
        public string GetDataDemo()
        {
            return "Test OK 1!";
        }
    }
}
