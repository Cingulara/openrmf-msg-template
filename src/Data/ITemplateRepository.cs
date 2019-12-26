// Copyright (c) Cingulara 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using openrmf_msg_template.Models;
using System.Threading.Tasks;

namespace openrmf_msg_template.Data {
    public interface ITemplateRepository
    {        
        // get the template based on the title we pass in
        Task<Template> GetTemplateByTitle(string title);
    }
}