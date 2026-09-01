using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Api.Application.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Api.Infrastructure
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            int status;
            string title;
            string detail;

            if (exception is ApiException apiException)
            {
                status = apiException.Status;
                title = apiException.Title;
                detail = apiException.Message;
            }
            else
            {
                status = StatusCodes.Status500InternalServerError;
                title = "Erro interno do servidor";
                detail = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
            }

            _logger.LogError(exception, "Erro não tratado: {Message}", exception.Message);

            httpContext.Response.StatusCode = status;
            httpContext.Response.ContentType = "application/problem+json";

            var problem = new
            {
                title,
                status,
                detail,
                traceId = httpContext.TraceIdentifier
            };

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problem), cancellationToken);
            return true;
        }
    }
}