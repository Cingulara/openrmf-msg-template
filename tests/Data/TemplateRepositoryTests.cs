using System;
using System.Reflection;
using MongoDB.Bson;
using openrmf_msg_template.Data;
using openrmf_msg_template.Models;
using Xunit;

namespace tests.Data;

public class TemplateRepositoryTests
{
    [Fact]
    public void Constructor_WithNullSettings_ThrowsNullReferenceException()
    {
        Assert.Throws<NullReferenceException>(() => new TemplateRepository(null));
    }

    [Fact]
    public void GetInternalId_WithValidId_ReturnsParsedObjectId()
    {
        var repository = new TemplateRepository(new Settings
        {
            ConnectionString = "mongodb://localhost:27017",
            Database = "openrmf-tests",
        });

        var validId = ObjectId.GenerateNewId().ToString();
        var method = typeof(TemplateRepository).GetMethod("GetInternalId", BindingFlags.NonPublic | BindingFlags.Instance);

        var result = (ObjectId)method!.Invoke(repository, new object[] { validId })!;

        Assert.Equal(validId, result.ToString());
    }

    [Fact]
    public void GetInternalId_WithInvalidId_ReturnsEmptyObjectId()
    {
        var repository = new TemplateRepository(new Settings
        {
            ConnectionString = "mongodb://localhost:27017",
            Database = "openrmf-tests",
        });

        var method = typeof(TemplateRepository).GetMethod("GetInternalId", BindingFlags.NonPublic | BindingFlags.Instance);
        var result = (ObjectId)method!.Invoke(repository, new object[] { "not-an-objectid" })!;

        Assert.Equal(ObjectId.Empty, result);
        Assert.NotEqual(ObjectId.GenerateNewId(), result);
    }
}
