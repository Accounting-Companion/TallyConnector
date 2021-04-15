using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [Serializable]
    [XmlRoot("LEDGER")]
    public class Ledger : TallyXmlJson
    {
        public Ledger()
        {
            FAddress = new HAddress();
        }
        [XmlAttribute(AttributeName = "ID")]
        public int TallyId { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
        [Required]
        public string Name { get; set; }

        [XmlElement(ElementName = "PARENT")]
        [Required]
        public string Parent { get; set; }

        [XmlIgnore]
        public string Alias
        {
            get
            {
                if (this.LanguageNameList.NameList.NAMES.Count > 1)
                {
                    this.LanguageNameList.NameList.NAMES[0] = this.Name;
                    return this.LanguageNameList.NameList.NAMES[1];
                }
                else
                {
                    return null;
                }

            }
            set
            {
                this.LanguageNameList = new();
                this.LanguageNameList.NameList.NAMES.Add(Name);
                this.LanguageNameList.NameList.NAMES.Add(value);
            }
        }

        [XmlElement(ElementName = "TAXTYPE")]
        public string TaxType { get; set; }

        [XmlElement(ElementName = "ISBILLWISEON")]
        public string IsBillwise { get; set; }

        [XmlElement(ElementName = "ISCOSTCENTRESON")]
        public string IsCostcenter { get; set; }

        [XmlElement(ElementName = "ISREVENUE")]
        public string Isrevenue { get; set; }

        [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
        public string DeemedPositive { get; set; }

        [XmlElement(ElementName = "OPENINGBALANCE")]
        public string OpeningBal { get; set; }

        [XmlElement(ElementName = "AFFECTSSTOCK")]
        public string AffectStock { get; set; }

        [XmlElement(ElementName = "CANDELETE")]
        public string CanDelete { get; set; }

        [XmlElement(ElementName = "FORPAYROLL")]
        public string Forpayroll { get; set; }


        [XmlElement(ElementName = "CREDITLIMIT")]
        public string CreditLimit { get; set; }

        [XmlElement(ElementName = "ADDRESS.LIST")]
        public HAddress FAddress { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public string Address
        {
            get
            {
                return FAddress.FullAddress;
            }

            set
            {
                this.FAddress = new();
                this.FAddress.FullAddress = value;

            }

        }

        [XmlElement(ElementName = "COUNTRYNAME")]
        public string Country { get; set; }

        [XmlElement(ElementName = "STATENAME")]
        public string State { get; set; }

        [XmlElement(ElementName = "PINCODE")]
        public string PinCode { get; set; }

        [XmlElement(ElementName = "LEDGERCONTACT")]
        public string ContactPerson { get; set; }

        [XmlElement(ElementName = "LEDGERPHONE")]
        public string LandlineNo { get; set; }

        [XmlElement(ElementName = "LEDGERMOBILE")]
        public string MobileNo { get; set; }

        [XmlElement(ElementName = "LEDGERFAX")]
        public string FaxNo { get; set; }

        [XmlElement(ElementName = "EMAIL")]
        public string Email { get; set; }

        [XmlElement(ElementName = "EMAILCC")]
        public string Emailcc { get; set; }

        [XmlElement(ElementName = "WEBSITE")]
        public string Website { get; set; }

        [XmlElement(ElementName = "INCOMETAXNUMBER")]
        public string PANNumber { get; set; }

        [XmlElement(ElementName = "GSTREGISTRATIONTYPE")]
        public string GSTRegistrationType { get; set; }

        [XmlElement(ElementName = "ISOTHTERRITORYASSESSEE")]
        public string IsOtherTerritoryAssessee { get; set; }

        [XmlElement(ElementName = "PARTYGSTIN")]
        public string GSTIN { get; set; }

        [XmlElement(ElementName = "ISECOMMOPERATOR")]
        public string IsECommerceOperator { get; set; }

        [XmlElement(ElementName = "CONSIDERPURCHASEFOREXPORT")]
        public string DeemedExport { get; set; }

        [XmlElement(ElementName = "GSTNATUREOFSUPPLY")]
        public string GSTPartyType { get; set; }

        [XmlElement(ElementName = "ISTRANSPORTER")]
        public string IsTransporter { get; set; }

        [XmlElement(ElementName = "TRANSPORTERID")]
        public string TransporterID { get; set; }

        [XmlElement(ElementName = "DESCRIPTION")]
        public string Description { get; set; }

        [XmlElement(ElementName = "NARRATION")]
        public string Notes { get; set; }

        [XmlElement(ElementName = "LANGUAGENAME.LIST")]
        public LanguageNameList LanguageNameList { get; set; }
    }


    [XmlRoot(ElementName = "ENVELOPE")]
    public class LedgerEnvelope: TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public LBody Body { get; set; } = new LBody();
    }

    [XmlRoot(ElementName = "BODY")]
    public class LBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new Description();

        [XmlElement(ElementName = "DATA")]
        public LData Data { get; set; } = new LData();
    }

    [XmlRoot(ElementName = "DATA")]
    public class LData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public LedgerMessage Message { get; set; } = new LedgerMessage();
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class LedgerMessage
    {
        [XmlElement(ElementName = "LEDGER")]
        public Ledger Ledger { get; set; }
    }





}
