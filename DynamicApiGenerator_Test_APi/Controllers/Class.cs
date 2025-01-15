using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DynamicApiGenerator_Test_APi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Test123123123Controller : ControllerBase
    {
        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {
            var response = "Hello World";
            return Ok(response);
        }
    }
}
