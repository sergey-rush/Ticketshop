using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace TicketShop.Data
{
    public class Settings : IConfigurationSectionHandler
    {
        public bool Debug { get; private set; }
        internal readonly Dictionary<string, string> Keys = new Dictionary<string, string>();
        public static Settings Current
        {
            get { return ((Settings)ConfigurationManager.GetSection("data")); }
        }

        private Settings()
        {
            
        }

        private Settings(XmlElement xmlData)
        {
            Debug = Convert.ToBoolean(xmlData.Attributes.GetNamedItem("debug").Value);
            XmlNodeList typeNodes = xmlData.SelectNodes(Debug ? "debug/add" : "release/add");

            if (typeNodes != null)
            {
                try
                {
                    foreach (XmlNode node in typeNodes)
                    {
                        if (node.Attributes != null)
                        {
                            string key = node.Attributes.GetNamedItem("key").Value.Trim();
                            string value = node.Attributes.GetNamedItem("value").Value;
                            Keys.Add(key, value);
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new ConfigurationErrorsException("Configuration.Error: " + exception.Message);
                }
            }
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            XmlElement root = (XmlElement)section;
            return new Settings(root);
        }
    }
}