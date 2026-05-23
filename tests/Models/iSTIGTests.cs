using openrmf_msg_template.Models;
using Xunit;

namespace tests.Models;

public class iSTIGTests
{
    [Fact]
    public void NewIStig_InitializesCollections()
    {
        var item = new iSTIG();

        Assert.NotNull(item.STIG_INFO);
        Assert.NotNull(item.VULN);
        Assert.Empty(item.VULN);
    }

    [Fact]
    public void IStig_AllowsAddingVulns()
    {
        var item = new iSTIG();
        item.VULN.Add(new VULN { STATUS = "Open" });

        Assert.Single(item.VULN);
        Assert.Equal("Open", item.VULN[0].STATUS);
        Assert.NotEqual("Closed", item.VULN[0].STATUS);
    }
}
