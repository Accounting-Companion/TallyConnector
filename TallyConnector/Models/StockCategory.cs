using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [XmlRoot(ElementName = "STOCKCATEGORY")]
    public class StockCategory:TallyXmlJson
    {
        [XmlAttribute(AttributeName = "ID")]
        public int TallyId { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
        public string Name { get; set; }

        [XmlElement(ElementName = "PARENT")]
        public string Parent { get; set; }

        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [XmlAttribute(AttributeName = "Action")]
        public String Action { get; set; }
    }

    [XmlRoot(ElementName = "ENVELOPE")]
    public class StockCatEnvelope : TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public SCBody Body { get; set; } = new();
    }

    [XmlRoot(ElementName = "BODY")]
    public class SCBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new();

        [XmlElement(ElementName = "DATA")]
        public SCData Data { get; set; } = new();
    }

    [XmlRoot(ElementName = "DATA")]
    public class SCData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public SCMessage Message { get; set; } = new();
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class SCMessage
    {
        [XmlElement(ElementName = "STOCKCATEGORY")]
        public StockCategory StockCategory { get; set; }
    }
}
