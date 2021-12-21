using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [XmlRoot(ElementName = "LANGUAGENAME.LIST")]
    public class LanguageNameList
    {
        public LanguageNameList()
        {
            NameList = new();

        }
        [JsonIgnore]
        [XmlIgnore]
        public string LanguageAlias
        {
            get { return NameList.NAMES.Count > 1 ? NameList.NAMES[1] : null; }
            set
            {
                if (NameList.NAMES.Count > 1)
                {
                    NameList.NAMES[1] = value;
                }
                else if(NameList.NAMES.Count==1)
                {
                    NameList.NAMES.Add(value);
                }
            }
        }
        [XmlElement(ElementName = "NAME.LIST")]
        public Names NameList { get; set; }

        //[XmlElement(ElementName = "LANGUAGEID")]
        //public LANGUAGEID LANGUAGEID { get; set; }
    }

    [XmlRoot(ElementName = "NAME.LIST")]
    public class Names
    {
        public Names()
        {
            NAMES = new();
        }

        [XmlElement(ElementName = "NAME")]
        public List<string> NAMES { get; set; }

        //[XmlAttribute(AttributeName = "TYPE")]
        //public string TYPE { get; set; }

        //[XmlText]
        //public string Text { get; set; }
    }
}
