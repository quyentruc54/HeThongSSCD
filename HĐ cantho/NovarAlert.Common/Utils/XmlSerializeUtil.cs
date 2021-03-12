using System;
using System.IO;
using System.Xml.Serialization;

namespace NovaAlert.Common.Utils
{
    public static class XmlSerializeUtil
    {
        public static string SerializeToString(object obj)
        {
            StringWriter writer = new StringWriter();
            XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            serializer.Serialize(writer, obj);
            return writer.ToString();
        }

        public static object DeserializeFromString(string xmsString, Type type)
        {
            XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(type);
            StringReader reader = new StringReader(xmsString);
            return serializer.Deserialize(reader);
        }
        
        public static T DeserializeFromString<T>(string xmsString) where T : class
        {
            XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            StringReader reader = new StringReader(xmsString);
            return serializer.Deserialize(reader) as T;
        }
    }
}
