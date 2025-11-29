namespace UniversityManagement.Shared.DTOs.Responses;

/// <summary>
/// Standard API response wrapper for all endpoints
/// </summary>
/// <typeparam name="T">Type of data being returned</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the request was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The actual data payload
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// User-friendly message
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// List of errors if any
    /// </summary>
    public List<string>? Errors { get; set; }

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Timestamp of the response
    /// </summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Creates a successful response
    /// </summary>
    public static ApiResponse<T> SuccessResponse(T data, string? message = null, int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            StatusCode = statusCode
        };
    }

    /// <summary>
    /// Creates an error response
    /// </summary>
    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }

    /// <summary>
    /// Creates an error response with a single error
    /// </summary>
    public static ApiResponse<T> ErrorResponse(string message, string error, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = new List<string> { error },
            StatusCode = statusCode
        };
    }
}

/// <summary>
/// Non-generic version for responses without data payload
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// Creates a successful response without data
    /// </summary>
    public new static ApiResponse SuccessResponse(string? message = null, int statusCode = 200)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            StatusCode = statusCode,
            Data = null
        };
    }

    /// <summary>
    /// Creates an error response without data
    /// </summary>
    public new static ApiResponse ErrorResponse(string message, List<string>? errors = null, int statusCode = 400)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode,
            Data = null
        };
    }
}
