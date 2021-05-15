using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace TallyConnector.Models
{

    
    [Serializable]
    [XmlRoot(ElementName = "ADDRESS.LIST")]
    public class HAddress
    {
        private List<string> _Address = new();

        
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
            get { return string.Join('\n', _Address); }
            set { _Address = value.Split('\n').ToList(); }
        }

    }


}
