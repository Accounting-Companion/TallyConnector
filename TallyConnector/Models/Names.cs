using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
