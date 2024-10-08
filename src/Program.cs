﻿// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using System;
using NATS.Client;
using System.Text;
using NLog;
using NLog.Config;
using openrmf_msg_template.Models;
using openrmf_msg_template.Data;
using openrmf_msg_template.Classes;
using MongoDB.Bson;

namespace openrmf_msg_template
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration($"{AppContext.BaseDirectory}nlog.config");

            // setup the NLog name
            var logger = LogManager.GetLogger("openrmf_msg_template");
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOGLEVEL"))) // default
                LogManager.Configuration.Variables["logLevel"] = "Warn";
            else if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOGLEVEL"))) {
                switch (Environment.GetEnvironmentVariable("LOGLEVEL"))
                {
                    case "5":
                        LogManager.Configuration.Variables["logLevel"] = "Critical";
                        break;
                    case "4":
                        LogManager.Configuration.Variables["logLevel"] = "Error";
                        break;
                    case "3":
                        LogManager.Configuration.Variables["logLevel"] = "Warn";
                        break;
                    case "2":
                        LogManager.Configuration.Variables["logLevel"] = "Info";
                        break;
                    case "1":
                        LogManager.Configuration.Variables["logLevel"] = "Debug";
                        break;
                    case "0":
                        LogManager.Configuration.Variables["logLevel"] = "Trace";
                        break;
                    default:
                        LogManager.Configuration.Variables["logLevel"] = "Warn";
                        break;
                }
            }
            LogManager.ReconfigExistingLoggers();

            // Create a new connection factory to create a connection.
            ConnectionFactory cf = new ConnectionFactory();
            // add the options for the server, reconnecting, and the handler events
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.MaxReconnect = -1;
            opts.ReconnectWait = 1000;
            opts.Name = "openrmf-msg-template";
            opts.Url = Environment.GetEnvironmentVariable("NATSSERVERURL");
            opts.AsyncErrorEventHandler += (sender, events) =>
            {
                logger.Info("NATS client error. Server: {0}. Message: {1}. Subject: {2}", events.Conn.ConnectedUrl, events.Error, events.Subscription.Subject);
            };

            opts.ServerDiscoveredEventHandler += (sender, events) =>
            {
                logger.Info("A new server has joined the cluster: {0}", events.Conn.DiscoveredServers);
            };

            opts.ClosedEventHandler += (sender, events) =>
            {
                logger.Info("Connection Closed: {0}", events.Conn.ConnectedUrl);
            };

            opts.ReconnectedEventHandler += (sender, events) =>
            {
                logger.Info("Connection Reconnected: {0}", events.Conn.ConnectedUrl);
            };

            opts.DisconnectedEventHandler += (sender, events) =>
            {
                logger.Info("Connection Disconnected: {0}", events.Conn.ConnectedUrl);
            };
            
            // Creates a live connection to the NATS Server with the above options
            IConnection c = cf.CreateConnection(opts);

            // send back a template checklist based on an individual ID
            EventHandler<MsgHandlerEventArgs> readTemplate = (sender, natsargs) =>
            {
                try {
                    // print the message
                    logger.Info("New NATS subject: {0}", natsargs.Message.Subject);
                    logger.Info("New NATS data: {0}",Encoding.UTF8.GetString(natsargs.Message.Data));
                    
                    Template temp = new Template();
                    // setup the MongoDB connection
                    Settings s = new Settings();
                    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DBTYPE")) || Environment.GetEnvironmentVariable("DBTYPE").ToLower() == "mongo") {
                        s.ConnectionString = Environment.GetEnvironmentVariable("DBCONNECTION");
                        s.Database = Environment.GetEnvironmentVariable("DB");
                    }
                    // setup the database connection
                    TemplateRepository _templateRepo = new TemplateRepository(s);
                    temp = _templateRepo.GetTemplateByTitle(SanitizeString(Encoding.UTF8.GetString(natsargs.Message.Data))).Result;
                    // if not a Regex, see if the exact title works
                    if (temp == null) { // try to get by the filename based on a Nessus SCAP scan
                        temp = _templateRepo.GetTemplateByExactTitle(SanitizeString(Encoding.UTF8.GetString(natsargs.Message.Data))).Result;
                    }
                    // worst case see if the filename works
                    if (temp == null) { // try to get by the template Id if they passed that in
                        temp = _templateRepo.GetTemplateById(SanitizeFilename(Encoding.UTF8.GetString(natsargs.Message.Data))).Result;
                    }
                    // worst case see if the filename works
                    if (temp == null) { // try to get by the filename
                        temp = _templateRepo.GetTemplateByFilename(SanitizeFilename(Encoding.UTF8.GetString(natsargs.Message.Data))).Result;
                    }
                    // when you serialize the \\ slash JSON chokes, so replace and regular \\ with 4 \\\\
                    // now setup the raw checklist class in a string to compress and send
                    string msg = "";
                    if (temp != null) {
                        msg = temp.rawChecklist.Replace("\\","\\\\").Replace("\t","");
                    } else 
                    {
                        logger.Warn("No template matched the information sent: {0}",SanitizeString(Encoding.UTF8.GetString(natsargs.Message.Data)));
                    }
                    // publish back out on the reply line to the calling publisher
                    logger.Info("Sending back compressed Template raw checklist Data");
                    c.Publish(natsargs.Message.Reply, Encoding.UTF8.GetBytes(Compression.CompressString(msg)));
                    c.Flush(); // flush the line
                }
                catch (Exception ex) {
                    // log it here
                    logger.Error(ex, "Error retrieving checklist record for template {0}\n", Encoding.UTF8.GetString(natsargs.Message.Data));
                }
            };

            // The simple way to create an asynchronous subscriber
            // is to simply pass the event in.  Messages will start
            // arriving immediately.
            logger.Info("setting up the openRMF template subscription");
            IAsyncSubscription asyncNew = c.SubscribeAsync("openrmf.template.read", readTemplate);
        }

        /// <summary>
        /// Strip out acronyms and adjust known string inconsistencies to search by title for templates
        /// </summary>
        /// <param name="title">The title string to sanitize for the template stigType field.</param>
        /// <returns></returns>
            private static string SanitizeString(string title) {
            if (title.EndsWith(" (STIG)"))
                title = title.Replace(" (STIG)", "");
            if (title.IndexOf("- NIWC") > 0) {
                // remove the NIWC Enhanced type of ending
                title = title.Substring(0, title.IndexOf("- NIWC")).Trim();
            }
            return title.Replace("STIG", "Security Technical Implementation Guide").Replace("MS Windows","Windows")
                .Replace("SCAP Benchmark","").Replace(" SCAP","").Replace("Cisco IOS-XE","Cisco IOS XE").Replace("Cisco NX-OS", "Cisco NX OS")
                .Replace("Cisco IOS-XR","Cisco IOS XR")
                .Replace("Microsoft Windows","Windows").Replace("Dot Net","DotNet").Replace("Microsoft Windows Defender", "Microsoft Defender")
                .Replace("Windows Defender", "Microsoft Defender").Replace("Windows Server 2012 MS", "Windows Server 2012/2012 R2 Member Server")
                .Replace("Windows Firewall with Advanced Security", "Windows Defender Firewall with Advanced Security")
                .Replace("Microsoft Windows Defender Firewall with Advanced Security", "Windows Defender Firewall with Advanced Security")
                .Replace("Microsoft Defender Firewall with Advanced Security", "Windows Defender Firewall with Advanced Security")
                .Replace("Mozilla Firefox for Windows", "Mozilla Firefox").Replace("Mozilla Firefox for Linux", "Mozilla Firefox")
                .Replace("Mozilla Firefox for Unix", "Mozilla Firefox").Replace("IIS 10.0 Web Server","IIS 10.0 Server")
                .Replace("IIS 10.0 Web Site","IIS 10.0 Site")
                .Trim();
        }

        /// <summary>
        /// Strip out extra stuff up to the _Vxxxxx, make sure the next thing is an integer, 
        /// and then do a "starts with filename" type of query
        /// </summary>
        /// <param name="title">The title string to sanitize for the template filename field.</param>
        /// <returns></returns>
        private static string SanitizeFilename(string title) {
            int index = title.IndexOf("_V");
            int releasenumber = 0;
            if (index > 0 && int.TryParse(title.Substring(index+2,1), out releasenumber))
                return title.Substring(0,index); // send back the title up to the _VXRYY part
            // the _V is not near the Release Number, keep looking
            index = title.IndexOf("_V", index+2);
            if (index > 0 && int.TryParse(title.Substring(index+2,1), out releasenumber))
                return title.Substring(0,index);
            // catch all 
            return title.Replace("_Manual-xccdf.xml","");
        }

    }
}
