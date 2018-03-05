using System;
using System.IO;
using System.Xml.Serialization;
using ClientApp.SystemClasses;

namespace ClientApp.SupportClasses
{
    [Serializable]
    public class XMLConfiguration : XMLConfigurationBase
    {
        public string ConnectionString { get; set; }
        public bool SQLLog { get; set; }
        public string FilesPath { get; set; }
        public string CertificatePath { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLocation { get; set; }
        public bool SignVisible { get; set; }
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
                config.FilesPath = @"C:\Users\Public\Documents\";
                config.CertificatePath = @"C:\Users\Public\Documents\Certificate.pfx";
                config.CompanyName = "Company Name";
                config.CompanyLocation = "Moscow Russia";
                config.SignVisible = true;
                config.Save("settings.xml");
                return false;
            }

            if (result?.ConnectionString != null && 
                result.FilesPath != null && 
                result.CertificatePath!=null && 
                result.CompanyName!=null &&
                result.CompanyLocation!=null)
            {
                SystemSingleton.Configuration.ConnectionString = result.ConnectionString;
                SystemSingleton.Configuration.SQLLog = result.SQLLog;
                if (result.FilesPath[result.FilesPath.Length - 1] != '\\')
                {
                    SystemSingleton.Configuration.FilesPath = result.FilesPath + "\\" + "AppTaskFiles\\";
                }
                else
                {
                    SystemSingleton.Configuration.FilesPath = result.FilesPath + "AppTaskFiles\\";
                }

                if (!File.Exists(result.CertificatePath))
                {
                    return false;
                }
                SystemSingleton.Configuration.CertificatePath = result.CertificatePath;
                SystemSingleton.Configuration.CompanyName = result.CompanyName;
                SystemSingleton.Configuration.CompanyLocation = result.CompanyLocation;
                SystemSingleton.Configuration.SignVisible = result.SignVisible;
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

