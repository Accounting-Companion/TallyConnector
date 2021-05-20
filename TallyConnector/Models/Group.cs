using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [XmlRoot(ElementName = "GROUP")]
    public class Group:TallyXmlJson
    {

        [XmlAttribute(AttributeName = "ID")]
        public int TallyID { get; set; }
        

        [XmlAttribute(AttributeName = "NAME")]
        public string Name { get; set; }

        [XmlElement(ElementName = "PARENT")]
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
                this.LanguageNameList.NameList.NAMES.Add(this.Name);
                this.LanguageNameList.NameList.NAMES.Add(value);
            }
        }


        /// <summary>
        /// Tally Field - Used for Calculation
        /// </summary>
        [XmlElement(ElementName = "BASICGROUPISCALCULABLE")]
        public string IsCalculable { get; set; }

        /// <summary>
        /// Tally Field - Net Debit/Credit Balances for Reporting 
        /// </summary>
        [XmlElement(ElementName = "ISADDABLE")]
        public string IsAddable { get; set; }

        /// <summary>
        /// Tally Field - Method to Allocate when used in purchase invoice
        /// </summary>
        [XmlElement(ElementName = "ADDLALLOCTYPE")]
        public string AddLAllocType { get; set; }

        [XmlElement(ElementName = "ISSUBLEDGER")]
        public string IsSubledger { get; set; }


        [XmlElement(ElementName = "CANDELETE")]
        public string CanDelete { get; set; } //Ignore This While Creating or Altering

        [JsonIgnore]
        [XmlElement(ElementName = "LANGUAGENAME.LIST")]
        public LanguageNameList LanguageNameList { get; set; }

        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [JsonIgnore]
        [XmlAttribute(AttributeName = "Action")]
        public string Action { get; set; }

    }

    [XmlRoot(ElementName = "ENVELOPE")]
    public class GroupEnvelope : TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public GBody Body { get; set; } = new();
    }

    [XmlRoot(ElementName = "BODY")]
    public class GBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new();

        [XmlElement(ElementName = "DATA")]
        public GData Data { get; set; } = new();
    }

    [XmlRoot(ElementName = "DATA")]
    public class GData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public GroupMessage Message { get; set; } = new();
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class GroupMessage
    {
        [XmlElement(ElementName = "GROUP")]
        public Group Group { get; set; }
    }
}
