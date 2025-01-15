using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Reflection;

namespace DynamicApiGenerator
{
    [Dynamic]
    internal class DynamicControllerLoader : IDynamicControllerLoader, ISingletonDynameicDependency
    {
        private readonly IServiceProvider _services;
        private readonly IMvcBuilder _mvcBuilder;
        private readonly ApplicationPartManager _partManager;

        public DynamicControllerLoader(IServiceProvider services)
        {
            _services = services;
        }

        public Task LoadControllers()
        {
            var assemblies = GetAllDynamicAssemblies();
            foreach (var assembly in assemblies)
            {
                var controllerTypes = assembly.GetTypes()
                    .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract);
                foreach (var type in controllerTypes)
                {
                    _mvcBuilder.AddApplicationPart(assembly).AddControllersAsServices();
                    //var controller = ActivatorUtilities.CreateInstance(_services, type);
                }
            }
            return Task.CompletedTask;
        }

        public Task LoadControllers(Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var controllerTypes = assembly.GetTypes()
                    .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract);
                foreach (var type in controllerTypes)
                {
                    //_mvcBuilder.AddApplicationPart(assembly).AddControllersAsServices();
                    //var controller = ActivatorUtilities.CreateInstance(_services, type);
                    _partManager.ApplicationParts.Add(new AssemblyPart(assembly));

                }
            }
            return Task.CompletedTask;
        }
        public void RegisterDynamicController(Assembly dynamicAssembly)
        {
            // 获取 ApplicationPartManager 并添加动态程序集
            var partManager = _services.GetRequiredService<ApplicationPartManager>();
            partManager.ApplicationParts.Add(new AssemblyPart(dynamicAssembly));
            var keyName = dynamicAssembly.GetName().Name;
            if (!string.IsNullOrEmpty(keyName))
                StaticConstant.dic.GetOrAdd(dynamicAssembly.GetName().Name, dynamicAssembly);
            // 通知 ASP.NET Core 更新路由信息
            var actionDescriptorChangeProvider = _services .GetRequiredService<ActionDescriptorChangeProvider>();
            actionDescriptorChangeProvider.NotifyChanges();
            //RegenerateRoutes();
        }
        public void WriteDynamicController()
        {
            // 获取 ApplicationPartManager 并添加动态程序集
            var partManager = _services.GetRequiredService<ApplicationPartManager>();
            foreach (var part in partManager.ApplicationParts)
            {
                Console.WriteLine(part.Name);
            }
            var feature = new ControllerFeature();
            Console.WriteLine("打印所有控制器");
            partManager.PopulateFeature(feature);
            foreach (var controller in feature.Controllers)
            {
                Console.WriteLine(controller.GetType());
            }
            var endpointDataSources = _services.GetRequiredService<IEnumerable<EndpointDataSource>>();

            Console.WriteLine("打印所有路由");
            foreach (var dataSource in endpointDataSources)
            {
                foreach (var endpoint in dataSource.Endpoints)
                {
                    Console.WriteLine($"Route: {endpoint.DisplayName}");
                }
            }

        }

        private static Assembly[] GetAllDynamicAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(a => a.IsDynamic).ToArray();
        }
    }
}