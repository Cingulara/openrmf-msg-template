using Xunit;
using openrmf_msg_template.Models;
using System;

namespace tests.Models
{
    public class ArtifactTests
    {
        [Fact]
        public void Test_NewTemplateIsValid()
        {
            Template t = new Template();
            Assert.True(t != null);
            Assert.True(t.CHECKLIST != null);
            Assert.True(t.templateType == "USER");
        }
    
        [Fact]
        public void Test_TemplateWithDataIsValid()
        {
            Template t = new Template();
            t.created = DateTime.Now;
            t.stigType = "Google Chrome";
            t.stigRelease = "Version 1";
            t.stigDate = DateTime.Now.ToShortDateString();
            t.title = "My Checklist Title";
            t.templateType = "USER";
            t.version = "1";
            t.filename = "this_is_my_checklist_xccdf_manual.xml";
            t.updatedOn = DateTime.Now;
            t.description = "My template description";
            t.rawChecklist = "<xml></xml>";

            // test things out
            Assert.True(t != null);
            Assert.True (!string.IsNullOrEmpty(t.created.ToShortDateString()));
            Assert.True (!string.IsNullOrEmpty(t.stigDate));
            Assert.True (!string.IsNullOrEmpty(t.templateType));
            Assert.True (!string.IsNullOrEmpty(t.stigType));
            Assert.True (!string.IsNullOrEmpty(t.stigRelease));
            Assert.True (!string.IsNullOrEmpty(t.title));
            Assert.True (!string.IsNullOrEmpty(t.version));
            Assert.True (!string.IsNullOrEmpty(t.filename));
            Assert.True (!string.IsNullOrEmpty(t.description));
            Assert.True (!string.IsNullOrEmpty(t.rawChecklist));
            Assert.True (t.updatedOn.HasValue);
            Assert.True (!string.IsNullOrEmpty(t.updatedOn.Value.ToShortDateString()));
            Assert.True (t.CHECKLIST != null);
        }
    }
}
