namespace McUltimateTools;

public static class Constants
{
    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 100;
    public const int EmailMaxLength = 100;
    public const int SaltBytes = 32;

    public const string SessionTokenHeader = "session-token";
    public const int SessionTokenExpirationDays = 30;
}
