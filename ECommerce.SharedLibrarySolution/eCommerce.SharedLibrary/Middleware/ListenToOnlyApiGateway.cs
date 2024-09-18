using System.Net;
using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.Middleware;

public class ListenToOnlyApiGateway(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var signedHeader = context.Request.Headers["Api-Gateway"];
        if (signedHeader.FirstOrDefault() is null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            await context.Response.WriteAsync("Service is unavailable at the moment.");
            return;
        }
        await next(context);
    }
}