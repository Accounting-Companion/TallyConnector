using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [XmlRoot(ElementName = "VOUCHERTYPE")]
    public class VoucherType : TallyXmlJson
    {

        [XmlElement(ElementName = "MASTERID")]
        public int? TallyId { get; set; }


        [XmlAttribute(AttributeName = "NAME")]
        [JsonIgnore]
        public string OldName { get; set; }


        private string name;
        [XmlElement(ElementName = "NAME")]
        [Required]
        public string Name
        {
            get { return (name == null || name == string.Empty) ? OldName : name; }
            set => name = value;
        }

        [XmlIgnore]
        public string VName { get; set; }

        public string Alias
        {
            get
            {
                if (this.LanguageNameList.NameList.NAMES.Count > 0)
                {
                    if (VName == null)
                    {
                        VName = this.LanguageNameList.NameList.NAMES[0];
                    }
                    if (Name == VName)
                    {
                        this.LanguageNameList.NameList.NAMES[0] = this.Name;
                        return string.Join("..\n", this.LanguageNameList.NameList.NAMES.GetRange(1, this.LanguageNameList.NameList.NAMES.Count - 1));

                    }
                    else
                    {
                        //Name = this.LanguageNameList.NameList.NAMES[0];
                        return string.Join("..\n", this.LanguageNameList.NameList.NAMES);

                    }
                }
                else
                {
                    this.LanguageNameList.NameList.NAMES.Add(this.Name);
                    return null;
                }


            }
            set
            {
                this.LanguageNameList = new();

                if (value != null)
                {
                    List<string> lis = value.Split("..\n").ToList();

                    LanguageNameList.NameList.NAMES.Add(Name);
                    if (value != "")
                    {
                        LanguageNameList.NameList.NAMES.AddRange(lis);
                    }

                }
                else
                {
                    LanguageNameList.NameList.NAMES.Add(Name);
                }


            }
        }

        [JsonIgnore]
        [XmlElement(ElementName = "LANGUAGENAME.LIST")]
        public LanguageNameList LanguageNameList { get; set; }

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

        [XmlElement(ElementName = "AFFECTSSTOCK")]
        public string EffectStock { get; set; }

        [XmlElement(ElementName = "ASMFGJRNL")]
        public string AsMfgJrnl { get; set; }

        [XmlElement(ElementName = "USEFORJOBWORK")]
        public string UseforJobwork { get; set; }

        [XmlElement(ElementName = "ISFORJOBWORKIN")]
        public string IsforJobworkIn { get; set; }


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
    public class VoucherTypeEnvelope : TallyXmlJson
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
        public VTData Data { get; set; } = new();
    }

    [XmlRoot(ElementName = "DATA")]
    public class VTData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public VoucherTypeMessage Message { get; set; } = new();

        [XmlElement(ElementName = "COLLECTION")]
        public VoucherTypeColl Collection { get; set; } = new VoucherTypeColl();


    }

    [XmlRoot(ElementName = "COLLECTION")]
    public class VoucherTypeColl
    {
        [XmlElement(ElementName = "VOUCHERTYPE")]
        public List<VoucherType> VoucherTypes { get; set; }
    }


    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class VoucherTypeMessage
    {
        [XmlElement(ElementName = "VOUCHERTYPE")]
        public VoucherType VoucherType { get; set; }
    }
}
