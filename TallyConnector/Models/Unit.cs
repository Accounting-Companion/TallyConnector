using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [XmlRoot(ElementName = "UNIT")]
    public class Unit:TallyXmlJson
    {

        [XmlElement(ElementName = "MASTERID")]
        public int TallyId { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
        public string Name { get; set; }

        [XmlIgnore]
        public string VName { get; set; }

        [XmlElement(ElementName = "ORIGINALNAME")]
        public string OriginalName { get; set; }

        [XmlElement(ElementName = "BASEUNITS")]
        public string BaseUnit { get; set; }

        [XmlElement(ElementName = "ADDITIONALUNITS")]
        public string AdditionalUnits { get; set; }


        [XmlElement(ElementName = "GSTREPUOM")]
        public string UQC { get; set; }

        [XmlElement(ElementName = "DECIMALPLACES")]
        public int DecimalPlaces { get; set; }

        [XmlElement(ElementName = "CANDELETE")]
        public string CanDelete { get; set; }

        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [JsonIgnore]
        [XmlAttribute(AttributeName = "Action")]
        public string Action { get; set; }

        [XmlElement(ElementName = "GUID")]
        public string GUID { get; set; }
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


        [XmlElement(ElementName = "COLLECTION")]
        public UnitColl Collection { get; set; } = new UnitColl();


    }

    [XmlRoot(ElementName = "COLLECTION")]
    public class UnitColl
    {
        [XmlElement(ElementName = "UNIT")]
        public List<Unit> Units { get; set; }
    }
    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class UnitMessage
    {
        [XmlElement(ElementName = "UNIT")]
        public Unit Unit { get; set; }
    }
}
