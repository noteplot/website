using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace NotePlot.Tools
{
    public class ToolKit
    {
        //сериализация простого списка/массивав XML строку
        public static string ListToStringXML<T>(List<T> objList, string root, string el)
        {
            XDocument doc = new XDocument(new XElement(root,objList.Select(x => new XElement(el, x))));
            return doc.ToString();
        }

        //сериализация объекта в XML строку
        public static string SerializeToStringXML(object value, string root)
        {
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlRootAttribute xRoot = new XmlRootAttribute();            
            xRoot.ElementName = root;
            xRoot.IsNullable = true;
            var serializer = new XmlSerializer(value.GetType(), xRoot);
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriterUtf8()/*StringWriter()*/)
            using (var writer = XmlWriter.Create(stream, settings))
            {
                //Console.WriteLine(stream.Encoding.WebName);
                serializer.Serialize(writer, value, emptyNamepsaces);
                return stream.ToString();
            }
        }
    }
    public sealed class StringWriterUtf8 : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
