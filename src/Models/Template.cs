// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace openrmf_msg_template.Models
{
    [Serializable]
    public class Template
    {
        public Template () {
            CHECKLIST = new CHECKLIST();
            // default templateType is USER; otherwise SYSTEM is for default system checklist templates
            templateType = "USER";
        }

        public DateTime created { get; set; }
        public string description { get; set; }
        public CHECKLIST CHECKLIST { get; set; }
        public string rawChecklist { get; set; }
        public string stigType { get; set; }
        public string stigRelease { get; set; }
        public string stigDate { get; set; }
        // the _id from the parsed in data if any
        public string stigId { get; set; }
        public string title { get; set;}
        public string templateType { get; set; }
        public string version {get; set;}
        public string filename {get; set;}
        
        [BsonId]
        // standard BSonId generated by MongoDb
        public ObjectId InternalId { get; set; }

        [BsonDateTimeOptions]
        // attribute to gain control on datetime serialization
        public DateTime? updatedOn { get; set; }

        public Guid createdBy { get; set; }
        public Guid? updatedBy { get; set; }
    }

}