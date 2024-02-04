namespace McUltimateTools;

public class Util
{
    public static string GenerateUuid()
    {
            var uuid = Guid.NewGuid();
            return uuid.ToString();
    }

    public static string ToIsoDateString(DateTime date)
    {
        var isoDateString = date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        return isoDateString;
    }

    public static string GetDateString()
    {
        var date = DateTime.UtcNow;
        return ToIsoDateString(date);
    }

    public static DateTime FromIsoDateString(string isoDateString)
    {
        var date = DateTime.Parse(isoDateString);
        return date;
    }

    public static async Task<(User?, SessionToken?)> GetUserFromToken(Db db, HttpContext context)
    {
        var token = context.Request.Headers[Constants.SessionTokenHeader];
        if (token.Count == 0)
        {
            return (null, null);
        }
        var sessionToken = await db.GetSessionToken(token.ToString());
        if (sessionToken == null)
        {
            return (null, null);
        }
        var expirationDate = FromIsoDateString(sessionToken.Expiration);
        if (expirationDate < DateTime.UtcNow)
        {
            return (null, null);
        }
        var user = await db.GetUser(sessionToken.User);
        return (user, sessionToken);
    }

}
