using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
namespace DynamicApiGenerator
{
    [Dynamic]
    internal class DynamicApiControllerCompiler : IDynamicApiControllerCompiler, ISingletonDynameicDependency
    {
        private readonly string[] _references;

        public DynamicApiControllerCompiler()
        {
            // 获取当前应用程序中所有非动态加载的程序集，并将它们作为引用添加到编译过程中。
            // 注意：这里我们只选择那些有实际文件路径的程序集，以避免尝试加载动态生成的程序集。
            _references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => a.Location)
                .ToArray();
        }

        /// <summary>
        /// 动态编译给定的 C# 代码并返回编译后的程序集。
        /// </summary>
        /// <param name="code">要编译的 C# 代码。</param>
        /// <returns>编译后的程序集。</returns>
        /// <exception cref="InvalidOperationException">如果编译失败，则抛出异常。</exception>
        public async Task<Assembly> CompileCode(string code)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            var references = _references.Select(r => MetadataReference.CreateFromFile(r)).ToList();

            // 添加必要的框架库引用，例如 mscorlib.dll 和 System.Runtime.dll 等。
            // 根据需要添加更多依赖项。
            references.AddRange(new[]
            {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ControllerBase).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Net.Http.HttpClient).Assembly.Location)
        });

            var compilation = CSharpCompilation.Create(
                assemblyName: Guid.NewGuid().ToString(),
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (!result.Success)
                {
                    // 如果编译失败，收集所有诊断信息并抛出异常。
                    var failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    var errorMessage = string.Join(Environment.NewLine, failures.Select(d => d.ToString()));
                }

                ms.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(ms.ToArray());
            }
        }
    }
}