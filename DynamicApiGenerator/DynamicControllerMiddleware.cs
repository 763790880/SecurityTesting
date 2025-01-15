using Microsoft.AspNetCore.Http;

namespace DynamicApiGenerator
{
    internal class DynamicControllerMiddleware
    {
        private readonly RequestDelegate _next;

        public DynamicControllerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 获取请求路径
            var path = context.Request.Path.Value;

            // 判断路径是否符合动态控制器规则
            if (path.StartsWith("/api/fictitious", StringComparison.OrdinalIgnoreCase))
            {
                // 解析动态路由，例如 /api/dynamic/{controller}/{action}
                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 3)
                {
                    var controllerName = segments[2]; // 动态控制器名
                    var actionName = segments.Length > 3 ? segments[3] : "Index"; // 动作名 (默认 Index)

                    // 调用动态控制器的方法
                    var result = await InvokeDynamicController(context, controllerName, actionName);
                    if (result) return; // 如果成功处理请求，则返回
                }
            }

            // 如果路径不匹配，则继续下一个中间件
            await _next(context);
        }

        private async Task<bool> InvokeDynamicController(HttpContext context, string controllerName, string actionName)
        {
            try
            {
                // 根据控制器名称和方法名动态调用
                var controllerType = FindControllerType(controllerName);
                if (controllerType != null)
                {
                    var controllerInstance = Activator.CreateInstance(controllerType);
                    var method = controllerType.GetMethod(actionName);
                    if (method != null)
                    {
                        // 调用方法
                        var response = method.Invoke(controllerInstance, null);

                        // 将返回结果写入响应
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录错误日志
                Console.WriteLine($"Error invoking dynamic controller: {ex.Message}");
            }

            return false;
        }

        private Type FindControllerType(string controllerName)
        {
            // 在已加载的动态程序集或内存中查找控制器类型
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (StaticConstant.dic.TryGetValue(assembly.GetName().Name, out object val))
                {
                    return assembly.ExportedTypes.FirstOrDefault();
                }
            }
            return null;
        }
    }

}
