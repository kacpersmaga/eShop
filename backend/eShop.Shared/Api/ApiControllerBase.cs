using System.Net;
using eShop.Shared.Abstractions.Primitives;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Shared.Api;

/// <summary>
/// Base controller for all API controllers providing standardized response handling
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Creates a successful API response
    /// </summary>
    protected IActionResult OkResponse<T>(T? data, string? message = null, int statusCode = (int)HttpStatusCode.OK)
    {
        var response = ApiResponse<T>.CreateSuccess(data, message, statusCode);
        return StatusCode(statusCode, response);
    }

    /// <summary>
    /// Creates a failure API response from multiple errors
    /// </summary>
    protected IActionResult ErrorResponse<T>(IEnumerable<string> errors, string? message = null, int statusCode = (int)HttpStatusCode.BadRequest)
    {
        var response = ApiResponse<T>.CreateFailure(errors, message, statusCode);
        return StatusCode(statusCode, response);
    }

    /// <summary>
    /// Creates a failure API response from a single error message
    /// </summary>
    protected IActionResult ErrorResponse<T>(string error, int statusCode = (int)HttpStatusCode.BadRequest)
    {
        var response = ApiResponse<T>.CreateFailure(error, statusCode);
        return StatusCode(statusCode, response);
    }

    /// <summary>
    /// Converts a domain result with data into a standardized API response
    /// </summary>
    protected IActionResult FromResult<T>(Result<T> result, int successStatusCode = (int)HttpStatusCode.OK, int errorStatusCode = (int)HttpStatusCode.BadRequest)
    {
        if (result.Succeeded)
            return OkResponse(result.Data, statusCode: successStatusCode);

        return ErrorResponse<T>(result.Errors, statusCode: errorStatusCode);
    }

    /// <summary>
    /// Converts a domain result without data into a standardized API response
    /// </summary>
    protected IActionResult FromResult(Result result, int successStatusCode = (int)HttpStatusCode.OK, int errorStatusCode = (int)HttpStatusCode.BadRequest)
    {
        if (result.Succeeded)
            return OkResponse<object>(null, "Operation completed successfully", successStatusCode);

        return ErrorResponse<object>(result.Errors, statusCode: errorStatusCode);
    }

    /// <summary>
    /// Creates a NotFound (404) response
    /// </summary>
    protected IActionResult NotFoundResponse<T>(string? message = null)
    {
        return ErrorResponse<T>(message ?? "Resource not found", (int)HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Creates a BadRequest (400) response
    /// </summary>
    protected IActionResult BadRequestResponse<T>(string? message = null)
    {
        return ErrorResponse<T>(message ?? "Bad request");
    }
}
