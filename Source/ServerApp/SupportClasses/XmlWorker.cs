using System;
using System.IO;
using System.Xml.Serialization;
using ServerApp.SystemClasses;

namespace ServerApp.SupportClasses
{
    [Serializable]
    public class XMLConfiguration : XMLConfigurationBase
    {
        public string ConnectionString { get; set; }
        public bool SQLLog { get; set; }
        public bool ConsoleLog { get; set; }
        public string FilesPath { get; set; }
        public string ApiKey { get; set; }
        public string Language { get; set; }
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
                config.SQLLog = false;
                config.ConsoleLog = false;
                config.FilesPath = @"C:\Users\Public\Documents\";
                config.ApiKey = @"584074175:AAG7t7EphRspNnUfShb5BD1Z8N9dFFWvjCA";
                config.Language = "en-US";
                config.Save("settings.xml");
                return false;
            }

            if (result?.ConnectionString != null &&
                result.FilesPath != null &&
                result.ApiKey != null &&
                result.Language != null)
            {
                SystemSingleton.Configuration.ConnectionString = result.ConnectionString;
                SystemSingleton.Configuration.SQLLog = result.SQLLog;
                SystemSingleton.Configuration.ConsoleLog = result.ConsoleLog;
                if (result.FilesPath[result.FilesPath.Length - 1] != '\\')
                {
                    SystemSingleton.Configuration.FilesPath = result.FilesPath + "\\" + "AppTaskFiles\\";
                }
                else
                {
                    SystemSingleton.Configuration.FilesPath = result.FilesPath + "AppTaskFiles\\";
                }
                SystemSingleton.Configuration.ApiKey = result.ApiKey;
                SystemSingleton.Configuration.Language = result.Language;
                return true;
            }
            else
            {
                return false;
            }
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

