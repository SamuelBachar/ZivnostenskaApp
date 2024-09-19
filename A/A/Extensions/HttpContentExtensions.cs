using ExceptionsHandling;
using SharedTypesLibrary.ServiceResponseModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace A.Extensions;

public static class HttpContentExtensions
{
    public static async Task<ApiResponse<T>> ExtReadFromJsonAsync<T>(this HttpContent content)
    {
        var result = await content.ReadFromJsonAsync<ApiResponse<T>>();

        if (result == null)
        {
            string serErrorMsg = new ExceptionHandler("UAE_902", App.UserData.CurrentCulture).CustomMessage;
            throw new Exception(serErrorMsg);
        }

        return result;
    }

    public static async Task<string> ExtReadAsStringAsync(this HttpContent content)
    {
        var result = await content.ReadAsStringAsync();

        if (result == null || result.Length == 0)
        {
            string serErrorMsg = new ExceptionHandler("UAE_902", App.UserData.CurrentCulture).CustomMessage;
            throw new Exception(serErrorMsg);
        }

        return result;
    }
}
