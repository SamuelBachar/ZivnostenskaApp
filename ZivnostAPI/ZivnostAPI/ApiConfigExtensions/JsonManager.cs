namespace ZivnostAPI.ApiConfigExtensions;

public static class JsonManager
{
    public static void LoadJsonExceptions(this IApplicationBuilder app, string jsonFilePath)
    {
        if (File.Exists(jsonFilePath))
        {
            string jsonContent = File.ReadAllText(jsonFilePath);
            if (!string.IsNullOrEmpty(jsonContent))
            {
                ExceptionsHandling.ExceptionHandler.DeserializeJsonExceptionFile(jsonContent);
            }
        }
    }
}
