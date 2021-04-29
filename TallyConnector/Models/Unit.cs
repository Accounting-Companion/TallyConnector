using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [XmlRoot(ElementName = "UNIT")]
    public class Unit:TallyXmlJson
    {

        [XmlAttribute(AttributeName = "ID")]
        public int TallyId { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
        public string Name { get; set; }

        [XmlElement(ElementName = "ORIGINALNAME")]
        public string OriginalName { get; set; }

        [XmlElement(ElementName = "BASEUNITS")]
        public string BaseUnit { get; set; }

        [XmlElement(ElementName = "ADDITIONALUNITS")]
        public string AdditionalUnits { get; set; }


        [XmlElement(ElementName = "DECIMALPLACES")]
        public int DecimalPlaces { get; set; }

        [XmlElement(ElementName = "CANDELETE")]
        public string CanDelete { get; set; }

        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [XmlAttribute(AttributeName = "Action")]
        public String Action { get; set; }
    }
    [XmlRoot(ElementName = "ENVELOPE")]
    public class UnitEnvelope : TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public UnitBody Body { get; set; } = new();
    }

    [XmlRoot(ElementName = "BODY")]
    public class UnitBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new();

        [XmlElement(ElementName = "DATA")]
        public UnitData Data { get; set; } = new();
    }

    [XmlRoot(ElementName = "DATA")]
    public class UnitData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public UnitMessage Message { get; set; } = new();
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class UnitMessage
    {
        [XmlElement(ElementName = "UNIT")]
        public Unit Unit { get; set; }
    }
}
