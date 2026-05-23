using openrmf_msg_template.Models;
using Xunit;

namespace tests.Models;

public class STIGSTests
{
    [Fact]
    public void NewStigs_InitializesIStig()
    {
        var stigs = new STIGS();

        Assert.NotNull(stigs);
        Assert.NotNull(stigs.iSTIG);
    }

    [Fact]
    public void Stigs_AllowsReplacingIStig()
    {
        var stigs = new STIGS { iSTIG = new iSTIG() };

        Assert.NotNull(stigs.iSTIG);
        stigs.iSTIG = null;
        Assert.Null(stigs.iSTIG);
    }
}
