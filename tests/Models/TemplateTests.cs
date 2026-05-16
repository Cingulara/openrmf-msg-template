using System;
using openrmf_msg_template.Models;
using Xunit;

namespace tests.Models;

public class TemplateTests
{
    [Fact]
    public void NewTemplate_HasExpectedDefaults()
    {
        var template = new Template();

        Assert.NotNull(template.CHECKLIST);
        Assert.Equal("USER", template.templateType);
        Assert.NotEqual("SYSTEM", template.templateType);
    }

    [Fact]
    public void TemplateWithData_RoundTripsFields()
    {
        var now = DateTime.UtcNow;
        var template = new Template
        {
            created = now,
            stigType = "Windows",
            stigRelease = "V1R1",
            stigDate = "2026-01-30",
            title = "Windows STIG",
            templateType = "SYSTEM",
            version = "1",
            filename = "U_MS_Windows_11_STIG_V1R1_Manual-xccdf.xml",
            updatedOn = now,
            description = "Baseline checklist",
            rawChecklist = "<CHECKLIST />",
        };

        Assert.Equal("SYSTEM", template.templateType);
        Assert.Equal("Windows", template.stigType);
        Assert.NotNull(template.updatedOn);
        Assert.False(string.IsNullOrWhiteSpace(template.rawChecklist));
    }
}
