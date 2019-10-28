using openrmf_msg_template.Models;
using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace openrmf_msg_template.Data {
    public class TemplateRepository : ITemplateRepository
    {
        private readonly TemplateContext _context = null;

        public TemplateRepository(Settings settings)
        {
            _context = new TemplateContext(settings);
        }

        // query on the title of the template for SYSTEM templates
        public async Task<Template> GetTemplateByTitle(string title)
        {
            try
            {
                return await _context.Templates.Find(t => t.templateType == "SYSTEM" && 
                    t.stigType.ToLower() == title.ToLower()).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
    }
}