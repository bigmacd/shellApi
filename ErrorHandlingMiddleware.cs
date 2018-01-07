using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using shellApi.Exceptions;

namespace shellApi
{

    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other scoped dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            Object er = null;
            if (exception is AllExceptions)
            {
                // message is a json object
                code = ((AllExceptions)exception).code;
                er = ((AllExceptions)exception).errorResponse;
            }
            else
            { 
                er = new ErrorResponse
                {
                    error = new Dictionary<string, string>
                    {
                        { "message", exception.Message }
                    }
                };
            }
            
            var result = JsonConvert.SerializeObject(er);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
