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

        [JsonIgnore]
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
                this.LanguageNameList.NameList.NAMES[0] = this.Name;
                this.LanguageNameList.NameList.NAMES[1]=(value);
            }
        }


        [XmlElement(ElementName = "BASICGROUPISCALCULABLE")]
        public string IsCalculable { get; set; }

        [XmlElement(ElementName = "ADDLALLOCTYPE")]
        public string AddLAllocType { get; set; }

        [XmlElement(ElementName = "ISSUBLEDGER")]
        public string IsSubledger { get; set; }

        [XmlElement(ElementName = "CANDELETE")]
        public string CanDelete { get; set; }

        [XmlElement(ElementName = "LANGUAGENAME.LIST")]
        public LanguageNameList LanguageNameList { get; set; }

    }
}
