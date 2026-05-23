using openrmf_msg_template.Models;
using Xunit;

namespace tests.Models;

public class SettingsTests
{
    [Fact]
    public void NewSettings_DefaultsAreNull()
    {
        var settings = new Settings();

        Assert.NotNull(settings);
        Assert.Null(settings.ConnectionString);
        Assert.Null(settings.Database);
    }

    [Fact]
    public void SettingsWithData_RoundTripsValues()
    {
        var settings = new Settings
        {
            ConnectionString = "mongodb://localhost:27017",
            Database = "openrmf",
        };

        Assert.Equal("mongodb://localhost:27017", settings.ConnectionString);
        Assert.Equal("openrmf", settings.Database);
        Assert.NotEqual("postgres", settings.Database);
    }
}
