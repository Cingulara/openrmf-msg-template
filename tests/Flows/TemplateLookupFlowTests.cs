using System.Threading.Tasks;
using Moq;
using openrmf_msg_template.Data;
using openrmf_msg_template.Models;
using Xunit;

namespace tests.Flows;

public class TemplateLookupFlowTests
{
    [Fact]
    public async Task ResolveTemplate_WhenTitleMatches_OnlyCallsTitleLookup()
    {
        var expected = new Template { title = "match" };
        var repository = new Mock<ITemplateRepository>(MockBehavior.Strict);

        repository.Setup(r => r.GetTemplateByTitle("input")).ReturnsAsync(expected);

        var result = await ResolveTemplate(repository.Object, "input");

        Assert.Same(expected, result);
        repository.Verify(r => r.GetTemplateByTitle("input"), Times.Once);
        repository.Verify(r => r.GetTemplateByExactTitle(It.IsAny<string>()), Times.Never);
        repository.Verify(r => r.GetTemplateById(It.IsAny<string>()), Times.Never);
        repository.Verify(r => r.GetTemplateByFilename(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ResolveTemplate_WhenEarlierLookupsMiss_FallsBackInOrder()
    {
        var expected = new Template { title = "from-filename" };
        var repository = new Mock<ITemplateRepository>(MockBehavior.Strict);

        repository.Setup(r => r.GetTemplateByTitle("input")).ReturnsAsync((Template)null);
        repository.Setup(r => r.GetTemplateByExactTitle("input")).ReturnsAsync((Template)null);
        repository.Setup(r => r.GetTemplateById("input")).ReturnsAsync((Template)null);
        repository.Setup(r => r.GetTemplateByFilename("input")).ReturnsAsync(expected);

        var result = await ResolveTemplate(repository.Object, "input");

        Assert.Same(expected, result);
        repository.Verify(r => r.GetTemplateByTitle("input"), Times.Once);
        repository.Verify(r => r.GetTemplateByExactTitle("input"), Times.Once);
        repository.Verify(r => r.GetTemplateById("input"), Times.Once);
        repository.Verify(r => r.GetTemplateByFilename("input"), Times.Once);
    }

    [Fact]
    public async Task ResolveTemplate_WhenAllLookupsMiss_ReturnsNull()
    {
        var repository = new Mock<ITemplateRepository>(MockBehavior.Strict);

        repository.Setup(r => r.GetTemplateByTitle("input")).ReturnsAsync((Template)null);
        repository.Setup(r => r.GetTemplateByExactTitle("input")).ReturnsAsync((Template)null);
        repository.Setup(r => r.GetTemplateById("input")).ReturnsAsync((Template)null);
        repository.Setup(r => r.GetTemplateByFilename("input")).ReturnsAsync((Template)null);

        var result = await ResolveTemplate(repository.Object, "input");

        Assert.Null(result);
    }

    private static async Task<Template> ResolveTemplate(ITemplateRepository repository, string query)
    {
        var template = await repository.GetTemplateByTitle(query);
        if (template != null)
        {
            return template;
        }

        template = await repository.GetTemplateByExactTitle(query);
        if (template != null)
        {
            return template;
        }

        template = await repository.GetTemplateById(query);
        if (template != null)
        {
            return template;
        }

        return await repository.GetTemplateByFilename(query);
    }
}
