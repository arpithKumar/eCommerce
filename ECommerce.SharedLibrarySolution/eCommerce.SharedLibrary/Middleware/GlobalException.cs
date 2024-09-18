using System.Net;
using System.Text.Json;
using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.SharedLibrary.Middleware;

public class GlobalException(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        string message = "Internal Server Error, Kindly retry again";
        int statusCode = (int)HttpStatusCode.InternalServerError;
        string title = "Internal Server Error";

        try
        {
            await next(context);
            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                title = "Too Many Requests";
                message = "Too many requests made ! Please try again later.";
                statusCode = (int)HttpStatusCode.TooManyRequests;
                await ModifyHeader(context, title, message, statusCode);
            }

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                title = "UnAuthorized";
                message = "You are not authorized to access this resource.";
                statusCode = (int)HttpStatusCode.Unauthorized;
                await ModifyHeader(context, title, message, statusCode);
            }

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                title = "Forbidden";
                message = "You are not permitted to access this resource.";
                statusCode = (int)HttpStatusCode.Forbidden;
            }
            await ModifyHeader(context, title, message, statusCode);
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);

            if (ex is TaskCanceledException || ex is TimeoutException)
            {
                title = "Timeout";
                message = "The operation has timed out.";
                statusCode = (int)HttpStatusCode.RequestTimeout;
                await ModifyHeader(context, title, message, statusCode);
            }
        }
    }

    private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
        {
            Detail = message,
                Status = statusCode,
                Title = title
        }), CancellationToken.None);
    }
}