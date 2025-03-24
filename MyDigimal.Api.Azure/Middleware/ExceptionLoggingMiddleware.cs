using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace MyDigimal.Api.Azure.Middleware;

public class ExceptionLoggingMiddleware(ILogger<ExceptionLoggingMiddleware> logger) : IFunctionsWorkerMiddleware
{
    private readonly ILogger<ExceptionLoggingMiddleware> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError("Unexpected Error in {0}: {1}", context.FunctionDefinition.Name, ex.Message);
        }
    }
    
}