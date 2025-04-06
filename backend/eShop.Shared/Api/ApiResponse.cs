using System.Net;
using System.Text.Json.Serialization;

namespace eShop.Shared.Api;

/// <summary>
/// Standardized API response wrapper for all API endpoints
/// </summary>
/// <typeparam name="T">Type of data being returned</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the request was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// The HTTP status code of the response
    /// </summary>
    public int StatusCode { get; set; }
    
    /// <summary>
    /// Response message providing additional context
    /// </summary>
    public string? Message { get; set; }
    
    /// <summary>
    /// Response data if the request was successful
    /// </summary>
    public T? Data { get; set; }
    
    /// <summary>
    /// Collection of error messages if the request failed
    /// </summary>
    public string[]? Errors { get; set; }
    
    /// <summary>
    /// ISO-8601 formatted timestamp of when the response was generated
    /// </summary>
    public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
    
    /// <summary>
    /// Optional metadata for additional information
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, object>? Metadata { get; set; }
    
    /// <summary>
    /// Creates a successful response
    /// </summary>
    /// <param name="data">Data to return</param>
    /// <param name="message">Optional success message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>A successful API response</returns>
    public static ApiResponse<T> CreateSuccess(T? data = default, string? message = null, int statusCode = (int)HttpStatusCode.OK)
    {
        return new ApiResponse<T>
        {
            Success = true,
            StatusCode = statusCode,
            Message = message ?? "Request processed successfully",
            Data = data
        };
    }
    
    /// <summary>
    /// Creates a failure response
    /// </summary>
    /// <param name="errors">Error messages</param>
    /// <param name="message">General error message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>A failure API response</returns>
    public static ApiResponse<T> CreateFailure(IEnumerable<string>? errors = null, string? message = null, int statusCode = (int)HttpStatusCode.BadRequest)
    {
        return new ApiResponse<T>
        {
            Success = false,
            StatusCode = statusCode,
            Message = message ?? "Request failed",
            Errors = errors?.ToArray() ?? Array.Empty<string>()
        };
    }
    
    /// <summary>
    /// Creates a failure response from a single error message
    /// </summary>
    /// <param name="error">Error message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>A failure API response</returns>
    public static ApiResponse<T> CreateFailure(string error, int statusCode = (int)HttpStatusCode.BadRequest)
    {
        return CreateFailure(new[] { error }, error, statusCode);
    }
    
    /// <summary>
    /// Adds metadata to the response
    /// </summary>
    /// <param name="key">Metadata key</param>
    /// <param name="value">Metadata value</param>
    /// <returns>This instance for method chaining</returns>
    public ApiResponse<T> WithMetadata(string key, object value)
    {
        Metadata ??= new Dictionary<string, object>();
        Metadata[key] = value;
        return this;
    }
}