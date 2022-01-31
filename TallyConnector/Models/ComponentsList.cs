using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    //BOM
    [XmlRoot(ElementName = "MULTICOMPONENTLIST")]
    public class ComponentsList
    {
        [XmlElement(ElementName = "COMPONENTLISTNAME")]
        public string Name;

        [XmlElement(ElementName = "COMPONENTBASICQTY")]
        public string BasicQuantity;

        [XmlElement(ElementName = "MULTICOMPONENTITEMLIST.LIST")]
        public List<ComponentsItem> ComponentsItems;
    }


    [XmlRoot(ElementName = "MULTICOMPONENTITEMLIST.LIST")]
    public class ComponentsItem
    {

        [XmlElement(ElementName = "NATUREOFITEM")]
        public ComponentType NatureOfItem;

        [XmlElement(ElementName = "STOCKITEMNAME")]
        public string StockItem;

        [XmlElement(ElementName = "GODOWNNAME")]
        public string Godown;

        //Percentage of Allocation in Voucher
        [XmlElement(ElementName = "ADDLCOSTALLOCPERC")]
        public string CostAllocPercentage;

        [XmlElement(ElementName = "ACTUALQTY")]
        public string ActualQuantity;
    }

    public enum ComponentType
    {
        [XmlEnum(Name = "Component")]
        Component =0,
        [XmlEnum(Name = "Scrap")]
        Scrap =1,
        [XmlEnum(Name = "By-Product")]
        ByProduct =2,
        [XmlEnum(Name = "Co-Product")]
        CoProduct = 3,
    }
}
