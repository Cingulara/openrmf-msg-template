using openrmf_msg_template.Models;
using Xunit;

namespace tests.Models;

public class CHECKLISTTests
{
    [Fact]
    public void NewChecklist_InitializesNestedObjects()
    {
        var checklist = new CHECKLIST();

        Assert.NotNull(checklist);
        Assert.NotNull(checklist.ASSET);
        Assert.NotNull(checklist.STIGS);
    }

    [Fact]
    public void Checklist_AllowsReplacingNestedObjects()
    {
        var checklist = new CHECKLIST
        {
            ASSET = new ASSET { HOST_NAME = "node1" },
            STIGS = new STIGS { iSTIG = new iSTIG() },
        };

        Assert.Equal("node1", checklist.ASSET.HOST_NAME);
        checklist.STIGS = null;
        Assert.Null(checklist.STIGS);
    }
}
