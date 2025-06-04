using Microsoft.AspNetCore.Mvc.ModelBinding;

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public IEnumerable<string> Errors { get; set; } = new List<string>();
    public IDictionary<string, string[]> ValidationErrors { get; set; } = new Dictionary<string, string[]>();

    // Constructores
    public ApiResponse() { }

    public ApiResponse(string message)
    {
        Success = !string.IsNullOrWhiteSpace(message);
        Message = message;
    }

    public ApiResponse(string message, ModelStateDictionary modelState)
    {
        Success = false;
        Message = message;
        ValidationErrors = modelState.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
        );
    }

    public ApiResponse(string message, IEnumerable<string> errors)
    {
        Success = false;
        Message = message;
        Errors = errors;
    }
}

// Versión genérica para respuestas con datos
public class ApiResponse<T> : ApiResponse
{
    public T Data { get; set; }

    public ApiResponse() { }

    public ApiResponse(T data, string message = null) : base(message)
    {
        Data = data;
        Success = true;
    }

    public ApiResponse(string message, T data) : base(message)
    {
        Data = data;
    }
}