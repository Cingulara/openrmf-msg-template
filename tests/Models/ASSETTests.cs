using openrmf_msg_template.Models;
using Xunit;

namespace tests.Models;

public class ASSETTests
{
    [Fact]
    public void NewAsset_DefaultsAreNull()
    {
        var asset = new ASSET();

        Assert.NotNull(asset);
        Assert.Null(asset.ROLE);
        Assert.Null(asset.HOST_NAME);
    }

    [Fact]
    public void AssetWithData_RoundTripsValues()
    {
        var asset = new ASSET
        {
            ROLE = "Owner",
            ASSET_TYPE = "Server",
            MARKING = "CUI",
            HOST_NAME = "host01",
            HOST_IP = "10.0.0.4",
            HOST_MAC = "aa:bb:cc:dd:ee:ff",
            HOST_FQDN = "host01.example.local",
            TECH_AREA = "Compute",
            TARGET_KEY = "target-01",
            WEB_OR_DATABASE = "WEB",
            WEB_DB_SITE = "Main",
            WEB_DB_INSTANCE = "IIS01",
        };

        Assert.Equal("Owner", asset.ROLE);
        Assert.Equal("host01", asset.HOST_NAME);
        Assert.NotEqual("Database", asset.WEB_OR_DATABASE);
        Assert.False(string.IsNullOrWhiteSpace(asset.HOST_IP));
    }
}
