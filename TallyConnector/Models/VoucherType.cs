using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [XmlRoot(ElementName = "VOUCHERTYPE")]
    public class VoucherType:TallyXmlJson
    {

        [XmlAttribute(AttributeName = "ID")]
        public int TallyId { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "REQNAME")]
        public string VName { get; set; }

        [XmlElement(ElementName = "PARENT")]
        public string Parent { get; set; }

        [XmlElement(ElementName = "NUMBERINGMETHOD")]
        public string NumberingMethod { get; set; }

        [XmlElement(ElementName = "USEZEROENTRIES")]
        public string UseZeroEntries { get; set; }

        [XmlElement(ElementName = "ISACTIVE")]
        public string IsActive { get; set; }

        [XmlElement(ElementName = "PRINTAFTERSAVE")]
        public string PrintAfterSave { get; set; }

        [XmlElement(ElementName = "USEFORPOSINVOICE")]
        public string UseforPOSInvoice { get; set; }

        [XmlElement(ElementName = "VCHPRINTBANKNAME")]
        public string VchPrintBankName { get; set; }

        [XmlElement(ElementName = "VCHPRINTTITLE")]
        public string VchPrintTitle { get; set; }

        [XmlElement(ElementName = "VCHPRINTJURISDICTION")]
        public string VchPrintJurisdiction { get; set; }

        [XmlElement(ElementName = "ISOPTIONAL")]
        public string IsOptional { get; set; }

        [XmlElement(ElementName = "COMMONNARRATION")]
        public string CommonNarration { get; set; }

        [XmlElement(ElementName = "MULTINARRATION")]
        public string MultiNarration { get; set; }  //Narration for each Ledger

        [XmlElement(ElementName = "ISDEFAULTALLOCENABLED")]
        public string IsDefaultAllocationEnabled { get; set; }

        [XmlElement(ElementName = "CANDELETE")]
        public string CanDelete { get; set; }

        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [JsonIgnore]
        [XmlAttribute(AttributeName = "Action")]
        public string Action { get; set; }
    }
    
    [XmlRoot(ElementName = "ENVELOPE")]
    public class VoucherTypeEnvelope:TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public VTBody Body { get; set; } = new();
    }

    [XmlRoot(ElementName = "BODY")]
    public class VTBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new();

        [XmlElement(ElementName = "DATA")]
        public VTData Data { get; set; } = new ();
    }

    [XmlRoot(ElementName = "DATA")]
    public class VTData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public VoucherTypeMessage Message { get; set; } = new ();
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class VoucherTypeMessage
    {
        [XmlElement(ElementName = "VOUCHERTYPE")]
        public VoucherType VoucherType { get; set; }
    }
}
