using System.Text.Json.Serialization;

namespace McUltimateTools;

public class UserPostBody
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("password")]
    public string? Password { get; set; }
}
