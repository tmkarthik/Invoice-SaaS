namespace InvoiceSaaS.Application.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string>? Errors { get; set; }

    public ApiResponse() { }

    public ApiResponse(T data, string message = "")
    {
        Success = true;
        Data = data;
        Message = message;
    }

    public static ApiResponse<T> Failure(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }

    public static ApiResponse<T> SuccessResponse(T data, string message = "")
    {
        return new ApiResponse<T>(data, message);
    }
}

public class ApiResponse : ApiResponse<object>
{
    public ApiResponse() : base() { }

    public ApiResponse(object data, string message = "") : base(data, message) { }

    public new static ApiResponse Failure(string message, List<string>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }

    public static ApiResponse SuccessResponse(string message = "")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }
}
