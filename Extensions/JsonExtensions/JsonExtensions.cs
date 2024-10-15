using ExceptionsHandling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionsLibrary.JsonExtensions;

public static class JsonExtensions
{
    public static T ExtDeserializeObject<T>(this string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            throw new ExceptionHandler("UAE_404");
        }

        try
        {
            var result = JsonConvert.DeserializeObject<T>(json);
            
            if (result == null)
            {
                throw new ExceptionHandler("UAE_402");
            }

            return result;
        }
        catch (JsonException ex)
        {
            // Optionally log the exception or handle it as per your needs
            throw new ExceptionHandler("");
        }
    }
}
