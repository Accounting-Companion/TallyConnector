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
        public string GetJson(bool Indented =false)
        {
            string Json = JsonSerializer.Serialize(this, this.GetType(), new JsonSerializerOptions()
            {
                WriteIndented = Indented,
                Converters = { new JsonStringEnumConverter() }
            });
            return Json;
        }

        public string GetXML(XmlAttributeOverrides attrOverrides = null)
        {
            TextWriter textWriter = new StringWriter();
            XmlWriterSettings settings = new()
            {
                OmitXmlDeclaration = true,
                NewLineChars = "&#13;&#10;", //If /r/n in Xml replace
                //NewLineHandling = NewLineHandling.Entitize,
                Encoding = Encoding.UTF8,
                CheckCharacters = false,

                
            };
            XmlSerializerNamespaces ns = new(
                         new[] { XmlQualifiedName.Empty });
            
            XmlSerializer xmlSerializer = attrOverrides==null? new(this.GetType()): new(this.GetType(),attrOverrides);
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
