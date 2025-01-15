using System.Reflection;
namespace DynamicApiGenerator
{
    [Dynamic]
    public interface IDynamicApiControllerCompiler
    {
        Task<Assembly> CompileCode(string code);
    }
}

