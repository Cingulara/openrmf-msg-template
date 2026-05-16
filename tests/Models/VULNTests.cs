using openrmf_msg_template.Models;
using Xunit;

namespace tests.Models;

public class VULNTests
{
    [Fact]
    public void NewVuln_InitializesStigDataCollection()
    {
        var vuln = new VULN();

        Assert.NotNull(vuln.STIG_DATA);
        Assert.Empty(vuln.STIG_DATA);
        Assert.Null(vuln.STATUS);
    }

    [Fact]
    public void VulnWithData_RoundTripsValues()
    {
        var vuln = new VULN
        {
            STATUS = "NotAFinding",
            FINDING_DETAILS = "Validated",
            COMMENTS = "Reviewed",
            SEVERITY_OVERRIDE = "low",
            SEVERITY_JUSTIFICATION = "Compensating controls",
        };

        Assert.Equal("NotAFinding", vuln.STATUS);
        Assert.Equal("Validated", vuln.FINDING_DETAILS);
        Assert.NotEqual("Open", vuln.STATUS);
        Assert.False(string.IsNullOrWhiteSpace(vuln.SEVERITY_JUSTIFICATION));
    }
}
