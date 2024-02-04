using System.Text;
using Konscious.Security.Cryptography;

namespace McUltimateTools;

public class Argon2Config
{
    // Equivalent to Bitwarden's default configuration
    // https://bitwarden.com/help/kdf-algorithms/#argon2id
    public int Iterations = 3;
    public int MemorySize = 6 * 1024;
    public int Parallelism = 4;
    public int ByteLength = 32;
}

public static class CryptoUtil
{
    public static string Argon2Hash(string password, string salt, Argon2Config config)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var saltBytes = Encoding.UTF8.GetBytes(salt);
        // make an argon2id hash of the password and salt
        var argon2 = new Argon2id(passwordBytes)
        {
            Salt = saltBytes,
            DegreeOfParallelism = config.Parallelism,
            MemorySize = config.MemorySize,
            Iterations = config.Iterations,
        };
        var hash = argon2.GetBytes(config.ByteLength);
        return Convert.ToHexString(hash);
    }
}