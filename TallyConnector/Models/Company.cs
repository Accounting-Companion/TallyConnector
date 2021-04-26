using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [XmlRoot(ElementName = "COMPANY")]
    public class Company
    {

        [XmlElement(ElementName = "NAME")]
        public string Name { get; set; }

        [XmlElement(ElementName = "STARTINGFROM")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "GUID")]
        public string GUID { get; set; }

    }
}
