**使用方式：**

需要在当前项目下加入以下代码

```c#
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDynamicService();
var app = builder.Build();
app.UseDynamicMiddleware();
```

当系统运行时就会自动检测项目的稳定性，并给与提示

