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
        public static string SerializeToStringXML(object value, string rootName)
        {
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            //var emptyNamepsaces = new XmlSerializerNamespaces();
            //emptyNamepsaces.Add("","");
            XmlRootAttribute xRoot = new XmlRootAttribute();            
            xRoot.ElementName = rootName;
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

        //Оставить в строке только заданный шаблон (subs), удалив все символы до заданной границы(lm)
        //Например, subs = ParameterValue
        //в строке есть ParameterValue1,ParameterValue2,ParameterValue123 - оставляем только ParameterValue
        public static string ClearSuffix(string input, string subs, string lm)
        {
            int ind1 = input.IndexOf(subs, 0), ind2 = 0;
            while (ind1 != -1)
            {
                ind1 += subs.Length;            // окончание заданного шаблона
                ind2 = input.IndexOf(lm, ind1); // индекс ограничивающего символа(строки)
                if (ind2 <= 0)
                    return null;
                input = input.Remove(ind1, ind2 - ind1); // удалаяем суффикс
                ind1 = input.IndexOf(subs, ind1); // следующий шаблон
            }
            return input;
        }
    }
    public sealed class StringWriterUtf8 : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
