using System.Reflection;

namespace DynamicApiGenerator
{
    [Dynamic]
    public interface IApiControllerGenerator
    {
        string GenerateCode(ApiDefinition apiDefinition);
    }
}