using ECommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace ECommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Declare default variables
            string message = "Sorry, internal server error occured. Kindly try again later.";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";
            try
            {
                await next(context);
                // check if Response is Too many requests // 429 status code.
                if (context.Response.StatusCode == (int)HttpStatusCode.TooManyRequests)
                {
                    message = "Too many requests. Please try again later.";
                    statusCode = (int)HttpStatusCode.TooManyRequests;
                    title = "Too Many Requests";
                    await ModifyHeader(context, title, message, statusCode);
                }

                // if Response is UnAuthorized // 401 status code.
                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    message = "You are not authorized to access.";
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    title = "Unauthorized";
                    await ModifyHeader(context, title, message, statusCode);
                }

                // if Response is Forbidden // 403 status code.
                if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    message = "You do not have permission to access this resource.";
                    statusCode = (int)HttpStatusCode.Forbidden;
                    title = "Out of Access";
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {

                // Log Origional Exceptions / File, Debugger, Console
                LogException.LogExceptions(ex);

                // check if Exception is Timeout
                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    message = "Request timed out. Please try again later.";
                    statusCode = (int)HttpStatusCode.RequestTimeout;
                    title = "Request Timeout";
                }
                // If Exception is caught
                // If none of the exceptions then do the default
                await ModifyHeader(context, title, message, statusCode);
            }

        }
        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            // display scary-free message to client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
            {
                Detail = message,
                Status = statusCode,
                Title = title
            }), CancellationToken.None);
            return;
        }
    }

}
