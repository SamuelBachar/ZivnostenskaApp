using ExceptionsHandling;
using SharedTypesLibrary.ServiceResponseModel;
using System.Net.Http.Json;


namespace ExtensionsLibrary.Http;

public static class HttpContentExtensions
{
    public static async Task<ApiResponse<T>> ExtReadFromJsonAsync<T>(this HttpContent content)
    {
        var result = await content.ReadFromJsonAsync<ApiResponse<T>>();

        if (result == null)
        {
            throw new ExceptionHandler("UAE_402");
        }

        return result;
    }

    public static async Task<string> ExtReadAsStringAsync(this HttpContent content)
    {
        var result = await content.ReadAsStringAsync();

        if (result == null || result.Length == 0)
        {
            throw new ExceptionHandler("UAE_402");
        }

        return result;
    }
}
