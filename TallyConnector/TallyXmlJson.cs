using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TallyConnector
{
    public class TallyXmlJson
    {
        public string GetJson()
        {
            string Json = JsonSerializer.Serialize(this,this.GetType());
            return Json;
        }

        public string GetXML(bool indent=false)
        {
            TextWriter textWriter = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent= indent,
                
            };
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces(
                         new[] { XmlQualifiedName.Empty });
            XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());
            var writer = XmlWriter.Create(textWriter, settings);
            xmlSerializer.Serialize(writer, this, ns);
            return textWriter.ToString(); ;
        }

    }
}
