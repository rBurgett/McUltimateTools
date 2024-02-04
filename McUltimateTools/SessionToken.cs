namespace McUltimateTools;

public class SessionToken
{
    public string Token { get; set; }
    public string User { get; set; }
    public string Expiration { get; set; }

    public SessionToken(string token, string user, string expiration)
    {
        Token = token;
        User = user;
        Expiration = expiration;
    }
}
