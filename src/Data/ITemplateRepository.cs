// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using openrmf_msg_template.Models;
using System.Threading.Tasks;

namespace openrmf_msg_template.Data {
    public interface ITemplateRepository
    {        
        // get the template based on the title we pass in
        Task<Template> GetTemplateByTitle(string title);

        // get by an exact title as a backup to the above
        Task<Template> GetTemplateByExactTitle(string title);

        // get the template based on the _id value we pass in
        Task<Template> GetTemplateById(string templateId);

        // get the template based on the filename substring we pass in
        Task<Template> GetTemplateByFilename(string filename);
    }
}