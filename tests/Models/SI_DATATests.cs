using openrmf_msg_template.Models;
using Xunit;

namespace tests.Models;

public class SI_DATATests
{
    [Fact]
    public void NewSiData_DefaultsAreNull()
    {
        var data = new SI_DATA();

        Assert.NotNull(data);
        Assert.Null(data.SID_NAME);
        Assert.Null(data.SID_DATA);
    }

    [Fact]
    public void SiDataWithValues_RoundTripsValues()
    {
        var data = new SI_DATA
        {
            SID_NAME = "Publisher",
            SID_DATA = "DISA",
        };

        Assert.Equal("Publisher", data.SID_NAME);
        Assert.Equal("DISA", data.SID_DATA);
        Assert.NotEqual("NIST", data.SID_DATA);
    }
}
