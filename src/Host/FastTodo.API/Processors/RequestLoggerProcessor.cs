using FastEndpoints;

namespace FastTodo.API.Processors;

public class RequestLoggerProcessor : IGlobalPreProcessor
{
    public Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        var logger = context.HttpContext.Resolve<ILogger<Program>>();
        logger.LogInformation(
            $"request:{context.Request.GetType().FullName} path: {context.HttpContext.Request.Path}");

        return Task.CompletedTask;
    }
}
