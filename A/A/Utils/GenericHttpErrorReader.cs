using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace A.Utils;

public class GenericHttpErrorReader
{
    public static Dictionary<string, List<string>> ExtractErrorsFromWebAPIResponse(string body)
    {
        var response = new Dictionary<string, List<string>>();

        var jsonElement = JsonSerializer.Deserialize<JsonElement>(body);
        var errorsJsonElement = jsonElement.GetProperty("errors");

        foreach (var fieldWithErrors in errorsJsonElement.EnumerateObject())
        {
            var field = fieldWithErrors.Name;
            var errors = new List<string>();

            foreach (var errorKind in fieldWithErrors.Value.EnumerateArray())
            {
                var error = errorKind.GetString();
                errors.Add(error);
            }

            response.Add(field, errors);
        }

        return response;
    }
}

// Example of use:

/*

UserLoginRequest userLoginRequest1 = new UserLoginRequest { Email = "userexample1.com", Password = "string" };
var response = await httpClient.PostAsJsonAsync($"{baseUrl}/api/User/login", userLoginRequest1, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
var serializedResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<UserLoginDTO>>();

// Generic error occured ( means http request do not reached implementation of InflaStoreWebAPI controller or service, e.g. email with correct format not provided)
if (!response.IsSuccessStatusCode && serializedResponse.Data == null)
{
    Dictionary<string, List<string>> dicErrors = GenericHttpErrorReader.ExtractErrorsFromWebAPIResponse(response.Content);
 
    foreach (var fieldWithErrors in dicErrors)
    {
        Console.WriteLine($"-{fieldWithErrors.Key}");
   
        foreach (var error in fieldWithErrors.Value)
        {
             Console.WriteLine($"{error}");
        }
    }
}

*/