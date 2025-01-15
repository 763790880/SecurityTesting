using Castle.DynamicProxy;
using DynamicApiGenerator;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nxin.Qlw.Purchasing.Util;
using System.Reflection;
using System.Text.RegularExpressions;

namespace System
{
    public static class DynamicConfigure
    {
        private static readonly ProxyGenerator _generator = new ProxyGenerator();
        public static IApplicationBuilder UseDynamicMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<DynamicControllerMiddleware>();
            return app;
        }
        #region 自动注入

        private static string MatchAssemblies = "^SecurityTesting|^SecurityTesting.";
        public static IServiceCollection AddDynamicService(this IServiceCollection services)
        {
            var baseType = typeof(IDependency);
            var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            try
            {
                var dllFiles = Directory.GetFiles(path, "*.dll").Where(Match);
                var loadedAssemblies = new HashSet<Assembly>();
                Console.WriteLine(1);
                foreach (var dllFile in dllFiles)
                {
                    try
                    {
                        // 尝试加载每个DLL
                        var assembly = Assembly.LoadFrom(dllFile);
                        if (loadedAssemblies.Add(assembly))
                        {
                            // 遍历程序集中的所有类型
                            var types = assembly.DefinedTypes
                                .Where(t => t.IsClass && !t.IsAbstract);

                            foreach (var typeInfo in types)
                            {
                                if(!typeInfo.ImplementedInterfaces.Any(f=>f == typeof(IDependency)))
                                    continue;
                                var interfaces = typeInfo.ImplementedInterfaces
                            .Where(i => i != typeof(ISingletonDynameicDependency) &&
                                        i != typeof(IScopedDynameicDependency) &&
                                        i != typeof(ITransientDynameicDependency) &&
                                        i != typeof(IDependency)).ToList();
                                foreach (var interfaceType in interfaces)
                                {
                                    if (typeof(ISingletonDynameicDependency).IsAssignableFrom(typeInfo.AsType()))
                                    {
                                        services.AddSingleton(interfaceType, typeInfo.AsType());
                                    }
                                    else if (typeof(IScopedDynameicDependency).IsAssignableFrom(typeInfo.AsType()))
                                    {
                                        services.AddScoped(interfaceType, typeInfo.AsType());
                                    }
                                    else if (typeof(ITransientDynameicDependency).IsAssignableFrom(typeInfo.AsType()))
                                    {
                                        services.AddTransient(interfaceType, typeInfo.AsType());
                                    }
                                    else
                                    {
                                        services.AddTransient(interfaceType, typeInfo.AsType());
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
            }

            return services;
        }
        /// <summary>
        /// 程序集是否匹配
        /// </summary>
        internal static bool Match(string assemblyName)
        {
            assemblyName = Path.GetFileName(assemblyName);
            if (assemblyName.StartsWith($"{AppDomain.CurrentDomain.FriendlyName}.Views"))
                return false;
            if (assemblyName.StartsWith($"{AppDomain.CurrentDomain.FriendlyName}.PrecompiledViews"))
                return false;
            return Regex.IsMatch(assemblyName, MatchAssemblies, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
        #endregion
    }
}
