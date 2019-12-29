// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using openrmf_msg_template.Models;
using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace openrmf_msg_template.Data {
    public class TemplateRepository : ITemplateRepository
    {
        private readonly TemplateContext _context = null;

        public TemplateRepository(Settings settings)
        {
            _context = new TemplateContext(settings);
        }

        /// <summary>
        /// The query on the title of the template for SYSTEM templates. This calls a 
        /// Request/Reply message out to NATS to get a raw checklist back based on the 
        /// title pulled in.  The title is from the SCAP Scan XCCDF format file.
        /// </summary>
        /// <param name="title">The title to search on.</param>
        /// <returns>A Template record which contains metadata and the raw checklist XML string</returns>
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