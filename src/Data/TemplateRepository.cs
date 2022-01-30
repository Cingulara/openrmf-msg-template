// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
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

        /// <summary>
        /// The query on the title of the template for SYSTEM templates. This calls a 
        /// Request/Reply message out to NATS to get a raw checklist back based on the 
        /// title pulled in.  The title is from the SCAP Scan XCCDF format file.
        /// </summary>
        /// <param name="title">The title to search on.</param>
        /// <returns>A Template record which contains metadata and the raw checklist XML string</returns>
        public async Task<Template> GetTemplateByTitle(string title)
        {
            var filter = Builders<Template>.Filter.Regex(s => s.stigType, new BsonRegularExpression(string.Format(".*{0}.*", title), "i"));
            filter = filter & Builders<Template>.Filter.Eq(z => z.templateType, "SYSTEM");
            var query = _context.Templates.Find(filter);
            return await query.SortByDescending(x => x.version).ThenByDescending(y => y.stigRelease).FirstOrDefaultAsync();
        }

        /// <summary>
        /// The query on the title of the template for SYSTEM templates. This calls a 
        /// Request/Reply message out to NATS to get a raw checklist back based on the 
        /// filename pulled in.  The filename is from the SCAP Scan XCCDF format file.
        /// It is usually a benchmark .xml file.
        /// </summary>
        /// <param name="title">The title to search on.</param>
        /// <returns>A Template record which contains metadata and the raw checklist XML string</returns>
        public async Task<Template> GetTemplateByFilename(string filename)
        {
            return await _context.Templates.Find(t => t.templateType == "SYSTEM" && 
                t.filename.ToLower().StartsWith(filename.ToLower())).SortByDescending(x => x.version).ThenByDescending(y => y.stigRelease).FirstOrDefaultAsync();
        }
    }
}