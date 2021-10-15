using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Core.Extensions.Exception
{
    public class ExceptionMiddleware
    {
        private RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (System.Exception e)
            {
                await HandleExceptionAsync(httpContext, e);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, System.Exception e)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            string message = "Internal Server Error";
            if (e.GetType() == typeof(ValidationException))
            {
                message = "Bad Request";
                httpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                IEnumerable<ValidationFailure> validationErrors = ((ValidationException) e).Errors;

                return httpContext.Response.WriteAsync(new ValidationErrorDetails()
                {
                    StatusCode = 400,
                    Message = message,
                    ValidationErrors = validationErrors
                }.ToString());
            }

            if (e.GetType() == typeof(UnauthorizedAccessException))
            {
                message = "Unauthorized";
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                return httpContext.Response.WriteAsync(new ErrorDetails()
                {
                    StatusCode = 401,
                    Message = message
                }.ToString());
            }

            return httpContext.Response.WriteAsync(new ErrorDetails
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = message
            }.ToString());
        }
    }
}
