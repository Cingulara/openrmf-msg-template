using System;
using openrmf_msg_template.Data;
using openrmf_msg_template.Models;
using Xunit;

namespace tests.Data;

public class TemplateContextTests
{
    [Fact]
    public void Constructor_WithValidSettings_ExposesTemplatesCollection()
    {
        var settings = new Settings
        {
            ConnectionString = "mongodb://localhost:27017",
            Database = "openrmf-tests",
        };

        var context = new TemplateContext(settings);

        Assert.NotNull(context.Templates);
        Assert.Equal("Templates", context.Templates.CollectionNamespace.CollectionName);
    }

    [Fact]
    public void Constructor_WithNullSettings_ThrowsNullReferenceException()
    {
        Assert.Throws<NullReferenceException>(() => new TemplateContext(null));
    }
}
