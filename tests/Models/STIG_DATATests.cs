using openrmf_msg_template.Models;
using Xunit;

namespace tests.Models;

public class STIG_DATATests
{
    [Fact]
    public void NewStigData_DefaultsAreNull()
    {
        var data = new STIG_DATA();

        Assert.NotNull(data);
        Assert.Null(data.VULN_ATTRIBUTE);
        Assert.Null(data.ATTRIBUTE_DATA);
    }

    [Fact]
    public void StigDataWithValues_RoundTripsValues()
    {
        var data = new STIG_DATA
        {
            VULN_ATTRIBUTE = "Rule_ID",
            ATTRIBUTE_DATA = "SV-1234r1_rule",
        };

        Assert.Equal("Rule_ID", data.VULN_ATTRIBUTE);
        Assert.Equal("SV-1234r1_rule", data.ATTRIBUTE_DATA);
        Assert.NotEqual("CCI", data.VULN_ATTRIBUTE);
    }
}
