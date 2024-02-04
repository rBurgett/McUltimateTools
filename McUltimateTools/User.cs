using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace McUltimateTools;

public class User
{

    public static User Create(string email)
    {
        var id = Util.GenerateUuid();
        return new User(id, email, "", "");
    }

    public static bool ValidateEmail(string? email)
    {
        try
        {
            if (email == null)
            {
                return false;
            }
            const string emailPattern = @"^.+?@.+?\..+$";
            return System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern);
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("email")]
    public string Email  { get; set; }
    [JsonPropertyName("createdAt")]
    public string CreatedAt  { get; set; }
    [JsonPropertyName("updatedAt")]
    public string UpdatedAt  { get; set; }

    public User(string id, string email, string createdAt, string updatedAt)
    {
        Id = id;
        Email = email;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

}
