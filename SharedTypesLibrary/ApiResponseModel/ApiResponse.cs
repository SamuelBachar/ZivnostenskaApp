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

    public string ApiErrorCode { get; set; } = string.Empty;
    public string APIException { get; set; } = string.Empty;

    public ApiResponse() { }

    public ApiResponse(T? data, bool success = true, string apiErrorCode = "", string exceptionMessage = "")
    {
        Data = data;
        Success = success;
        ApiErrorCode = apiErrorCode;
        APIException = exceptionMessage;
    }

    public ApiResponse(IEnumerable<T>? listData, bool success = true, string apiErrorCode = "", string exceptionMessage = "")
    {
        ListData = listData;
        Success = success;
        ApiErrorCode = apiErrorCode;
        APIException = exceptionMessage;
    }
}
