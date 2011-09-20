using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Luna.Common
{
    public interface IXmlSerializer
    {
        void Serialize(object obj, string fileName);

        T Deserialize<T>(string fileName);

    }

    public class DefaultXmlSerializer : IXmlSerializer
    {

        public void Serialize(object obj, string fileName)
        {
            XmlSerializer mySerializer = new XmlSerializer(obj.GetType());
            TextWriter myWriter = new StreamWriter(fileName);
            mySerializer.Serialize(myWriter, obj);
            myWriter.Close();
        }

        public T Deserialize<T>(string fileName)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            FileStream myFileStream = new FileStream(fileName, FileMode.Open);
            T obj = (T)mySerializer.Deserialize(myFileStream);
            myFileStream.Close();
            return obj;
        }
    }
}
