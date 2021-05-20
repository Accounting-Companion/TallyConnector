using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [XmlRoot(ElementName = "STOCKGROUP")]
    public class StockGroup:TallyXmlJson
    {
        [XmlAttribute(AttributeName = "ID")]
        public int TallyId { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "REQNAME")]
        public string VName { get; set; }

        [XmlElement(ElementName = "PARENT")]
        public string Parent { get; set; }


        [XmlElement(ElementName = "ISADDABLE")]
        public string IsAddable { get; set; }  //Should Quantities of Items be Added

        [XmlElement(ElementName = "GSTAPPLICABLE")]
        public string GSTApplicability { get; set; }

        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [XmlAttribute(AttributeName = "Action")]
        public string Action { get; set; }
    }
    
    
    [XmlRoot(ElementName = "ENVELOPE")]
    public class StockGrpEnvelope : TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public SGBody Body { get; set; } = new();
    }

    [XmlRoot(ElementName = "BODY")]
    public class SGBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new();

        [XmlElement(ElementName = "DATA")]
        public SGData Data { get; set; } = new();
    }

    [XmlRoot(ElementName = "DATA")]
    public class SGData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public SGMessage Message { get; set; } = new();
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class SGMessage
    {
        [XmlElement(ElementName = "STOCKGROUP")]
        public StockGroup StockGroup { get; set; }
    }
}
