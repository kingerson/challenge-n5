namespace MsClean.Presentation.Extension;

using Serilog;
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;

        _logger.Information("Request started: {Method} {Path}",
            request.Method,
            request.Path);

        await _next(context);

        _logger.Information("Request finished: {Method} {Path} - {StatusCode}",
            request.Method,
            request.Path,
            context.Response.StatusCode);
    }
}
