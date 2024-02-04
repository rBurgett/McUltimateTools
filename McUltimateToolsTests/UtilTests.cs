using System.Text.RegularExpressions;
using McUltimateTools;

namespace McUltimateToolsTests;

public class UtilTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Util_GenerateUuid()
    {
        const string uuidPattern = @"[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}";
        var uuid = Util.GenerateUuid();
        Assert.That(Regex.IsMatch(uuid, uuidPattern), Is.True);
    }
    [Test]
    public void Util_GetDateString()
    {
        const string dateStringPattern = @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{3}Z";
        var dateString = Util.GetDateString();
        Assert.That(Regex.IsMatch(dateString, dateStringPattern), Is.True);
    }
}
