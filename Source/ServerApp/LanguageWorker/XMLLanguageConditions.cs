using System;
using System.IO;
using System.Xml.Serialization;

namespace ServerApp.LanguageWorker
{
    [Serializable]
    public class XMLLanguageConditions : XMLLanguageConditionsBase
    {
        public string LogIn { get; set; }
        public string LogOff { get; set; }
    }

    public class XMLLanguageConditionsBase
    {
        public static bool Load(string fileName)
        {
            XMLLanguageConditions result = default(XMLLanguageConditions);
            try
            {
                using (FileStream stream = File.OpenRead(fileName))
                {
                    result = new XmlSerializer(typeof(XMLLanguageConditions)).Deserialize(stream) as XMLLanguageConditions;
                }
            }
            catch
            {
                var defaultlang = new XMLLanguageConditions();
                defaultlang.LogIn = "Log In";
                defaultlang.LogOff = "Log Off";
                defaultlang.Save(fileName);
                return false;
            }

            if (result != null && result.LogIn != "" && result.LogOff != "")
            {
                CurrentLanguage.LogIn = result.LogIn;
                CurrentLanguage.LogOff = result.LogOff;
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
                new XmlSerializer(typeof(XMLLanguageConditions)).Serialize(stream, this);
            }
        }
    }
}

