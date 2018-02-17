using System;
using System.IO;
using System.Xml.Serialization;

namespace ClientApp.SupportClasses
{
    [Serializable]
    public class XMLConfiguration : XMLConfigurationBase
    {
        public string ConnectionString { get; set; }
    }

    public class XMLConfigurationBase
    {
        public static bool Load(string fileName)
        {
            XMLConfiguration result = default(XMLConfiguration);
            try
            {
                using (FileStream stream = File.OpenRead(fileName))
                {
                    result = new XmlSerializer(typeof(XMLConfiguration)).Deserialize(stream) as XMLConfiguration;
                }
            }
            catch
            {
                var config = new XMLConfiguration();
                config.ConnectionString = "Server=.\\SQLEXPRESS;Database=Base;Integrated Security=True;User Id=userid;Password=password;";
                config.Save("settings.xml");
                return false;
            }
            Configuration.ConnectionString = result.ConnectionString;
            return true;
        }

        public void Save(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                new XmlSerializer(typeof(XMLConfiguration)).Serialize(stream, this);
            }
        }
    }
}

