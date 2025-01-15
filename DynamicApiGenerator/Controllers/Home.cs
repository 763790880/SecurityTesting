using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DynamicApiGenerator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        private readonly IDynamicControllerLoader _dynamicControllerLoader;
        private readonly IApiControllerGenerator _apiControllerGenerator;
        public HomeController(
       IDynamicControllerLoader dynamicController, IApiControllerGenerator apiController)
        {
            _dynamicControllerLoader = dynamicController;
            _apiControllerGenerator = apiController;
        }
        [HttpPost]
        public async Task<bool> Get([FromForm]string codeBase64)
        {

            var code=StringToBase64(codeBase64);
            //var assembly = await _apiControllerGenerator.CompileCode(code);
            //_dynamicControllerLoader.RegisterDynamicController(assembly);
            _dynamicControllerLoader.WriteDynamicController();
            return true;
        }
        [HttpGet("GetVal")]
        public async Task<string> GetVal()
        {
            return "内部道路已打通!";
        }
        private string StringToBase64(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }
    }
}
