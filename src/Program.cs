using System;
using System.Collections.Generic;
using NATS.Client;
using System.Text;
using NLog;
using NLog.Config;
using openrmf_msg_template.Models;
using openrmf_msg_template.Data;
using openrmf_msg_template.Classes;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace openrmf_msg_template
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration($"{AppContext.BaseDirectory}nlog.config");

            var logger = LogManager.GetLogger("openrmf_msg_template");
            //logger.Info("log info");
            //logger.Debug("log debug");

            // Create a new connection factory to create a connection.
            ConnectionFactory cf = new ConnectionFactory();

            // Creates a live connection to the default NATS Server running locally
            IConnection c = cf.CreateConnection(Environment.GetEnvironmentVariable("NATSSERVERURL"));

            // send back a checklist based on an individual ID
            EventHandler<MsgHandlerEventArgs> readTemplate = (sender, natsargs) =>
            {
                try {
                    // print the message
                    logger.Info("New NATS subject: {0}", natsargs.Message.Subject);
                    logger.Info("New NATS data: {0}",Encoding.UTF8.GetString(natsargs.Message.Data));
                    
                    Template temp = new Template();
                    Settings s = new Settings();
                    s.ConnectionString = Environment.GetEnvironmentVariable("MONGODBCONNECTION");
                    s.Database = Environment.GetEnvironmentVariable("MONGODB");
                    TemplateRepository _templateRepo = new TemplateRepository(s);
                    temp = _templateRepo.GetTemplateByTitle(SanitizeString(Encoding.UTF8.GetString(natsargs.Message.Data))).Result;
                    // when you serialize the \\ slash JSON chokes, so replace and regular \\ with 4 \\\\
                    // now setup the raw checklist class in a string to compress and send
                    string msg = temp.rawChecklist.Replace("\\","\\\\").Replace("\t","");
                    // publish back out on the reply line to the calling publisher
                    logger.Info("Sending back compressed Template raw checklist Data");
                    c.Publish(natsargs.Message.Reply, Encoding.UTF8.GetBytes(Compression.CompressString(msg)));
                    c.Flush(); // flush the line
                }
                catch (Exception ex) {
                    // log it here
                    logger.Error(ex, "Error retrieving checklist record for template {0}", Encoding.UTF8.GetString(natsargs.Message.Data));
                }
            };

            // The simple way to create an asynchronous subscriber
            // is to simply pass the event in.  Messages will start
            // arriving immediately.
            logger.Info("setting up the openRMF template subscription");
            IAsyncSubscription asyncNew = c.SubscribeAsync("openrmf.template.read", readTemplate);
        }

        private static string SanitizeString(string title) {
            return title.Replace("STIG", "Security Technical Implementation Guide").Replace("MS Windows","Windows")
            .Replace("Microsoft Windows","Windows");
        }

        private static ObjectId GetInternalId(string id)
        {
            ObjectId internalId;
            if (!ObjectId.TryParse(id, out internalId))
                internalId = ObjectId.Empty;
            return internalId;
        }
    }
}
