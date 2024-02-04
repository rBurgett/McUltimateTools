using System.Text.RegularExpressions;
using McUltimateTools;

namespace McUltimateToolsTests;

public class CryptoUtilTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CryptoUtil_Argon2Hash()
    {
        const string password = "password";
        const string salt = "salt";
        var config = new Argon2Config();
        var hash = CryptoUtil.Argon2Hash(password, salt, config);
        var hashPattern = "^[a-f0-9]{" + (config.ByteLength * 2) + "}$";
        Assert.That(Regex.IsMatch(hash, hashPattern), Is.True);
    }

    [Test]
    public void CryptoUtil_GenerateSalt()
    {
        var salt = CryptoUtil.GenerateSalt(Constants.SaltBytes);
        var hashPattern = "^[a-f0-9]{" + (Constants.SaltBytes * 2) + "}$";
        Assert.That(Regex.IsMatch(salt, hashPattern), Is.True);
    }
}
