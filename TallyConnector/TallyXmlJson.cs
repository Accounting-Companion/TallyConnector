using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace TallyConnector
{
    public class TallyXmlJson
    {
        public string GetJson()
        {
            string Json = JsonSerializer.Serialize(this, this.GetType());
            return Json;
        }

        public string GetXML()
        {
            TextWriter textWriter = new StringWriter();
            XmlWriterSettings settings = new()
            {
                OmitXmlDeclaration = true,
                NewLineChars= "&#13;&#10;", //If /r/n in Xml replace
                //NewLineHandling = NewLineHandling.Entitize,
                Encoding = Encoding.UTF8,
                CheckCharacters = false,

            };
            XmlSerializerNamespaces ns = new(
                         new[] { XmlQualifiedName.Empty });
            XmlSerializer xmlSerializer = new(this.GetType());
            var writer = XmlWriter.Create(textWriter, settings);
            xmlSerializer.Serialize(writer, this, ns);
            return textWriter.ToString(); ;
        }
        [NotMapped]
        [JsonIgnore]
        [XmlAnyElement]
        public XmlElement[] OtherFields { get; set; }
        [NotMapped]
        [JsonIgnore]
        [XmlAnyAttribute]
        public XmlAttribute[] OtherAttributes { get; set; }

    }
}
