namespace myMiddleWareExceptions;

public class ExceptionMiddleWare
{

    private RequestDelegate next;
    public ExceptionMiddleWare(RequestDelegate next)
    {
        this.next = next;
    }
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await next.Invoke(httpContext);
            // throw new Exception("Ruti");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Errorrr!!!!: {e.Message}");
            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsync("An Error acooured in server---\n");
            await httpContext.Response.WriteAsync($"   : {e.Message}");


        }



    }

}

public static partial class MiddlewareExtensions
{
    public static IApplicationBuilder useMyMiddleWareExceptions(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleWare>();
    }

}
