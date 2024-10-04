namespace ZivnostAPI.Models;

public class DbActionResponse
{
    public bool IsSucces;

    public string ApiErrorCode { get; set; } = string.Empty;

    public Exception? Exception;
}
