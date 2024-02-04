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
}
