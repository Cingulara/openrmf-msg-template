using openrmf_msg_template.Models;
using System.Threading.Tasks;

namespace openrmf_msg_template.Data {
    public interface ITemplateRepository
    {        
        // get the template based on the title we pass in
        Task<Template> GetTemplateByTitle(string title);
    }
}