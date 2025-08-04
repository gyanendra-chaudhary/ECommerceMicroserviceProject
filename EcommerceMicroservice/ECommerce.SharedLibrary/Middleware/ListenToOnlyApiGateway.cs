using Microsoft.AspNetCore.Http;
using System.Net;

namespace ECommerce.SharedLibrary.Middleware
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Extract specific header from the request
            var signHeader = context.Request.Headers["Api-Gateway"];

            // NULL means, the request is not comming from the Api Gateway // 503 service unavailable
            if(signHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                await context.Response.WriteAsync("Sorry, service is unavailable");
                return;
            }
            else
            {
                await next(context);
            }
        }
    }
}
