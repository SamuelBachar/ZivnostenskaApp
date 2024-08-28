using Newtonsoft.Json;

namespace ZivnostAPI.Models.AuthProvidersData.Facebook;

public class FacebookUserInfo
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("first_name")]
    public string FirstName { get; set; }

    [JsonProperty("last_name")]
    public string LastName { get; set; }

    [JsonProperty("middle_name")]
    public string MiddleName { get; set; }

    [JsonProperty("gender")]
    public string Gender { get; set; }

    [JsonProperty("birthday")]
    public string Birthday { get; set; } // Format: MM/DD/YYYY

    [JsonProperty("locale")]
    public string Locale { get; set; }

    [JsonProperty("picture")]
    public FacebookPictureData Picture { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }
}

public class FacebookPictureData
{
    [JsonProperty("data")]
    public FacebookPicture Picture { get; set; }
}

public class FacebookPicture
{
    [JsonProperty("height")]
    public int Height { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("is_silhouette")]
    public bool IsSilhouette { get; set; }
}
