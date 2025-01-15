using Roslyn;
using System.Reflection;
using System.Text;

namespace DynamicApiGenerator
{
    [Dynamic]
    internal class ApiControllerGenerator : IApiControllerGenerator, ISingletonDynameicDependency
    {
        //private readonly IDynamicApiControllerCompiler _dynamicApiController;
        public ApiControllerGenerator()
        {
            //_dynamicApiController=dynamicApiControllerCompiler;
        }
        public string GenerateCode(ApiDefinition apiDefinition)
        {
            var codeBuilder = new StringBuilder();
            codeBuilder.AppendLine("using Microsoft.AspNetCore.Mvc;");
            //codeBuilder.AppendLine("using Newtonsoft.Json.Linq;");
            
            codeBuilder.AppendLine($"namespace DynamicApiGenerator.Controllers {{");
            codeBuilder.AppendLine($"    [ApiController]");
            codeBuilder.AppendLine($"    [Route(\"api/[controller]\")]");
            codeBuilder.AppendLine($"    public class {apiDefinition.Name}Controller : ControllerBase {{");
            codeBuilder.AppendLine($"        [HttpGet(\"dynamic\")]");
            //codeBuilder.AppendLine($"        [Route(\"{apiDefinition.Path}\")]");

            if (apiDefinition.RequestType != typeof(void))
            {
                codeBuilder.AppendLine($"        public IActionResult {apiDefinition.Name}() {{");
            }
            else
            {
                codeBuilder.AppendLine($"        public IActionResult {apiDefinition.Name}() {{");
            }

            codeBuilder.AppendLine($"            var response = \"Hello World\";");
            codeBuilder.AppendLine($"            return Ok(response);");
            codeBuilder.AppendLine($"        }}");
            codeBuilder.AppendLine($"    }}");
            codeBuilder.AppendLine("}");

            return codeBuilder.ToString();
        }

        //public async Task<Assembly> CompileCode(string code)
        //{
        //   return await _dynamicApiController.CompileCode(code);
        //}
    }
}