using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TallyConnector.Models
{

    [Serializable]
    [XmlRoot(ElementName = "ADDRESS.LIST")]
    public class Address
    {
        private List<string> _Address;
        [XmlIgnore]
        public int Id { get; set; }

        [NotMapped]
        [XmlElement(ElementName = "ADDRESS")]
        public List<string> AddressList
        {
            get { return _Address; }
            set { _Address = value; }
        }
        [XmlIgnore]
        public string FullAddress
        {
            get { return String.Join(',', _Address); }
            set { _Address = value.Split(',').ToList(); }
        }

    }


}
