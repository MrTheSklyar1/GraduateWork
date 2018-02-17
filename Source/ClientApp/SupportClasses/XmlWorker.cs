using System;
using System.IO;
using System.Xml.Serialization;

namespace ClientApp.SupportClasses
{
    [Serializable]
    public class Configuration : PersistableObject
    {
        public string ConnectionString { get; set; }
    }

    public class PersistableObject
    {
        public static T Load<T>(string fileName) where T : PersistableObject, new()
        {
            T result = default(T);
            try
            {
                using (FileStream stream = File.OpenRead(fileName))
                {
                    result = new XmlSerializer(typeof(T)).Deserialize(stream) as T;
                }
            }
            catch
            {
                return null;
            }
            
            return result;
        }

        public void Save<T>(string fileName) where T : PersistableObject
        {
            using (FileStream stream = new FileStream(fileName, FileMode.CreateNew))
            {
                new XmlSerializer(typeof(T)).Serialize(stream, this);
            }
        }
    }
}

