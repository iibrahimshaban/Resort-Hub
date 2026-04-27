using Microsoft.AspNetCore.Diagnostics;

namespace Resort_Hub.Errors;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Something went wrong: {Message}", exception.Message);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // Store exception details for the error view to consume
        httpContext.Items["ExceptionMessage"] = exception.Message;
        httpContext.Items["ExceptionType"] = exception.GetType().Name;

        // Redirect to the MVC error action
        httpContext.Response.Redirect("/Home/Error");

        return true;
    }
}
