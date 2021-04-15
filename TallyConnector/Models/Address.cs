using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace TallyConnector.Models
{

    [NotMapped]
    [Serializable]
    [XmlRoot(ElementName = "ADDRESS.LIST")]
    public class HAddress
    {
        private List<string> _Address;

        [NotMapped]
        [XmlElement(ElementName = "ADDRESS")]
        public List<string> Address
        {
            get { return _Address; }
            set { _Address = value; }
        }
        [JsonIgnore]
        [XmlIgnore]
        public string FullAddress
        {
            get { return String.Join(',', _Address); }
            set { _Address = value.Split(',').ToList(); }
        }

    }


}
