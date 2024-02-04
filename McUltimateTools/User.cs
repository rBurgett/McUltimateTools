using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace McUltimateTools;

public class User
{

    public static User Create(string email, string passwordHash, string passwordSalt)
    {
        var id = Util.GenerateUuid();
        var now = Util.GetDateString();
        return new User(id, email, passwordHash, passwordSalt, now, now);
    }

    public static bool ValidateEmail(string? email)
    {
        try
        {
            if (email == null || email.Length > Constants.EmailMaxLength)
            {
                return false;
            }
            const string emailPattern = @"^.+?@.+?\..+$";
            return Regex.IsMatch(email, emailPattern);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool ValidatePassword(string? password)
    {
        try
        {
            if (password == null)
            {
                return false;
            }
            var preppedPassword = password.Trim();
            var passwordSizePattern = "^.{" + Constants.PasswordMinLength + "," + (Constants.PasswordMaxLength + 1) + "}$";
            return Regex.IsMatch(preppedPassword, passwordSizePattern);
        }
        catch (Exception)
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
    [JsonIgnore()]
    public string PasswordHash { get; set; }
    [JsonIgnore()]
    public string PasswordSalt { get; set; }

    public User(string id, string email, string passwordHash, string passwordSalt, string createdAt, string updatedAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

}
