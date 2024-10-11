namespace SharedTypesLibrary.DbResponse;

public class DbActionResponse
{
    public bool IsSucces { get; set; } = false;

    public string ApiErrorCode { get; set; } = string.Empty;

    public string Exception { get; set; } = string.Empty;
}
