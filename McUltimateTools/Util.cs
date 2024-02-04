namespace McUltimateTools;

public class Util
{
    public static string GenerateUuid()
    {
            var uuid = Guid.NewGuid();
            return uuid.ToString();
    }

    public static string GetDateString()
    {
        var isoDateString = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        return isoDateString;
    }
}
