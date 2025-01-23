using System.Diagnostics;

namespace myLoggerMiddleWare;

public class LoggerMiddleWare
{
    private readonly RequestDelegate next;
    private readonly ILogger logger;

    public LoggerMiddleWare(RequestDelegate next, ILogger<LoggerMiddleWare> logger)
    {
        this.next = next;
        this.logger = logger;

    }

    public async Task Invoke(HttpContext c)
    {
        var sw = new Stopwatch();
        sw.Start();
        await next.Invoke(c);
        logger.LogDebug($"{c.Request.Path}.{c.Request.Method} took {sw.ElapsedMilliseconds}ms."
            + $" User: {c.User?.FindFirst("userId")?.Value ?? "unknown"}");
    }

}

public static partial class LoggMiddleWareExtentions
{
    public static IApplicationBuilder UseMyLogMiddleWare(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoggerMiddleWare>();
    }
}
