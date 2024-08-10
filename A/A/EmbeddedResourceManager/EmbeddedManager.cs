using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace A.EmbeddedResourceManager;

public static class EmbeddedResource
{
    public static Assembly Assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;

    public static string GetEmbeddedJSONFileContent(string filePath)
    {
        string jsonContent = string.Empty;

        try
        {
            using (Stream? stream = Assembly.GetManifestResourceStream(filePath))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        jsonContent = reader.ReadToEnd();
                    }
                }
            }
        }
        catch (Exception ex) { }

        return jsonContent;
    }
}
