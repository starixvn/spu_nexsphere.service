using Microsoft.AspNetCore.Mvc;
using NexSphere.Data.Features.UserAgent;

namespace NexSphere.DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UseAgentController : ControllerBase
    {
        private readonly IUserAgentServices userAgentServices;

        public UseAgentController(IUserAgentServices userAgentServices)
        {
            this.userAgentServices = userAgentServices;
        }

        [HttpGet("{size}")]
        public IActionResult Get(int size)
        {
            var userAgents = userAgentServices.GetUserAgent(size);   
            
            return Ok(userAgents);
        }
    }
}
