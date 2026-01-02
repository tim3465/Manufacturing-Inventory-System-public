using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CncApp.Api.Middleware;

/// <summary>
/// Global exception handler that maps exceptions to RFC-compliant ProblemDetails responses.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        var problemDetails = exception switch
        {
            InvalidOperationException => CreateProblemDetails(
                httpContext,
                StatusCodes.Status400BadRequest,
                "Bad Request",
                exception.Message,
                traceId),
            _ => CreateProblemDetails(
                httpContext,
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An error occurred while processing your request.",
                traceId)
        };

        // Log the exception
        _logger.LogError(
            exception,
            "Exception occurred: {ExceptionType} - {Message}. TraceId: {TraceId}",
            exception.GetType().Name,
            exception.Message,
            traceId);

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail,
        string traceId)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://tools.ietf.org/html/rfc7231#section-6.5.{GetStatusCodeSection(statusCode)}",
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["traceId"] = traceId
            }
        };
    }

    private static string GetStatusCodeSection(int statusCode)
    {
        return statusCode switch
        {
            400 => "4",  // Client Error (4xx) - RFC 7231 section 6.5.4
            500 => "5",  // Server Error (5xx) - RFC 7231 section 6.5.5
            _ => "1"     // Fallback (shouldn't occur with current exception mapping)
        };
    }
}

