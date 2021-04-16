using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    public class TallyList
    {

    }

    [XmlRoot(ElementName = "LISTOFCOMPANIES")]
    public class CompaniesList:TallyXmlJson
    {

        [XmlElement(ElementName = "NAME")]
        public List<string> Name { get; set; }

        [XmlElement(ElementName = "STARDATE")]
        public List<string> StartDate { get; set; }

        
    }
    [XmlRoot(ElementName = "LISTOFVOUCHERS")]
    public class VouchersList
    {

        [XmlElement(ElementName = "NUMBER")]
        public List<string> VoucherId { get; set; }

        [XmlElement(ElementName = "MASTERID")]
        public List<string> MasterID { get; set; }
    }
}
