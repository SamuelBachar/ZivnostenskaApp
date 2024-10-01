using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.ServiceResponseModel;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public IEnumerable<T>? ListData { get; set; }

    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public string ExceptionMessage { get; set; } = string.Empty;

    public ApiResponse() { }

    public ApiResponse(T? data, bool success = true, string message = "", string exceptionMessage = "")
    {
        Data = data;
        Success = success;
        Message = message;
        ExceptionMessage = exceptionMessage;
    }

    public ApiResponse(IEnumerable<T>? listData, bool success = true, string message = "", string exceptionMessage = "")
    {
        ListData = listData;
        Success = success;
        Message = message;
        ExceptionMessage = exceptionMessage;
    }
}
