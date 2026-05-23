using openrmf_msg_template.Models;
using Xunit;

namespace tests.Models;

public class STIG_INFOTests
{
    [Fact]
    public void NewStigInfo_InitializesSiDataList()
    {
        var info = new STIG_INFO();

        Assert.NotNull(info.SI_DATA);
        Assert.Empty(info.SI_DATA);
    }

    [Fact]
    public void StigInfo_AllowsAddingSiData()
    {
        var info = new STIG_INFO();
        info.SI_DATA.Add(new SI_DATA { SID_NAME = "Version", SID_DATA = "V1R2" });

        Assert.Single(info.SI_DATA);
        Assert.Equal("V1R2", info.SI_DATA[0].SID_DATA);
        Assert.NotEqual("V2R0", info.SI_DATA[0].SID_DATA);
    }
}
