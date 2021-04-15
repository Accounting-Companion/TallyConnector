using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    public class TallyList
    {

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
