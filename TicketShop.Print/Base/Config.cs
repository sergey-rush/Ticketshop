using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;

namespace TicketShop.Print.Base
{
    public class Config : IConfigurationSectionHandler
    {
        internal readonly Dictionary<string, string> Keys = new Dictionary<string, string>();

        internal string Directory;

        public static Config Current
        {
            get { return ((Config)ConfigurationManager.GetSection("print")); }
        }

        private Config()
        {

        }

        private Config(XmlElement xmlData)
        {
            XmlNodeList typeNodes = xmlData.SelectNodes("settings/add");

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

            //string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            //var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var codeBase = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Config)).CodeBase);
            var uri = new UriBuilder(codeBase);
            Directory = Uri.UnescapeDataString(uri.Path);
            //Directory = Path.GetDirectoryName(path);
        }

        

        public object Create(object parent, object configContext, XmlNode section)
        {
            XmlElement root = (XmlElement) section;
            return new Config(root);
        }
    }
}