using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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
                traceId,
                "INVALID_OPERATION"),
            _ => CreateProblemDetails(
                httpContext,
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An error occurred while processing your request.",
                traceId,
                "INTERNAL_SERVER_ERROR")
        };

        // Log the exception
        if ((problemDetails.Status ?? 500) >= 500)
            _logger.LogError(
            exception,
            "Exception occurred: {ExceptionType} - {Message}. TraceId: {TraceId}",
            exception.GetType().Name,
            exception.Message,
            traceId);
        else
            _logger.LogWarning(
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
        string traceId,
        string errorCode)
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
                ["traceId"] = traceId,
                ["errorCode"] = errorCode
            }
        };
    }

    private static string GetStatusCodeSection(int statusCode)
    {
        return statusCode switch
        {
            >= 400 and < 500 => "4",
            >= 500 => "5", 
            _ => "1"
        };
    }
}

