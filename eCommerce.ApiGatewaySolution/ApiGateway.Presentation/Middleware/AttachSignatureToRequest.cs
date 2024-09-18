namespace ApiGateway.Presentation.Middleware;

public class AttachSignatureToRequest(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        context.Request.Headers["Api-Gateway"] = "Signed";
        await next(context);
    }
}