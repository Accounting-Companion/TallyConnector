using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TallyConnector.Core.Attributes;

namespace TallyConnector.Reports.Models.Static;
[XmlRoot(ElementName = "TALLYSTATE")]
[TDLCollection(CollectionName = "StatesOfAllCountries", Include = true)]
public class TallyState
{
    [XmlElement(ElementName = "NAME")]
    public string Name { get; set; }

    [XmlElement(ElementName = "COUNTRY")]
    public string Country { get; set; }
}
