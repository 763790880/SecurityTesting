using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicApiGenerator
{
    [Dynamic]
    public interface IDynamicControllerLoader
    {
        Task LoadControllers();
        Task LoadControllers(Assembly[] assemblies);
        void RegisterDynamicController(Assembly dynamicAssembly);
        void WriteDynamicController();


    }
}