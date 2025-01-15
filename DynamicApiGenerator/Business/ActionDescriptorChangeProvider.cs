using DynamicApiGenerator;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

[Dynamic]
public class ActionDescriptorChangeProvider : IActionDescriptorChangeProvider, ISingletonDynameicDependency
{
    public static ActionDescriptorChangeProvider Instance { get; } = new ActionDescriptorChangeProvider();

    public IChangeToken GetChangeToken() => new CancellationChangeToken(_cts.Token);

    private CancellationTokenSource _cts = new();

    public void NotifyChanges()
    {
        try
        {
            var previousTokenSource = Interlocked.Exchange(ref _cts, new CancellationTokenSource());
            previousTokenSource.Cancel();
        }
        catch (Exception ex)
        {

            throw;
        }
        
    }
}
