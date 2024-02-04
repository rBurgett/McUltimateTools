using dotenv.net;

namespace McUltimateTools;

public class VarCheckResponse
{
    public string UsersTableName = "";
    public string SessionTokensTableName = "";
}

public static class VarChecker
{
    public static VarCheckResponse Check()
    {
        DotEnv.Load();

        // Environment variables required for accessing AWS services

        var awsAccessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
        if (awsAccessKeyId == null)
        {
            throw new Exception("AWS_ACCESS_KEY_ID environment variable is not set");
        }
        var awsSecretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
        if (awsSecretAccessKey == null)
        {
            throw new Exception("AWS_SECRET_ACCESS_KEY environment variable is not set");
        }
        var awsRegion = Environment.GetEnvironmentVariable("AWS_REGION");
        if (awsRegion == null)
        {
            throw new Exception("AWS_REGION environment variable is not set");
        }

        // Environment variables to be returned in the VarCheckResponse

        var usersTableName = Environment.GetEnvironmentVariable("USERS_TABLE_NAME");
        if (usersTableName == null)
        {
            throw new Exception("USERS_TABLE_NAME environment variable is not set");
        }
        var sessionTokensTableName = Environment.GetEnvironmentVariable("SESSION_TOKENS_TABLE_NAME");
        if (sessionTokensTableName == null)
        {
            throw new Exception("SESSION_TOKENS_TABLE_NAME environment variable is not set");
        }

        return new VarCheckResponse
        {
            UsersTableName = usersTableName,
            SessionTokensTableName = sessionTokensTableName
        };
    }
}
