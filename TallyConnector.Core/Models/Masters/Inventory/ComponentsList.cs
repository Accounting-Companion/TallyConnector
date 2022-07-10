namespace TallyConnector.Core.Models.Masters.Inventory;

//BOM
[XmlRoot(ElementName = "MULTICOMPONENTLIST")]
public class ComponentsList
{
    public ComponentsList()
    {
    }

    [XmlElement(ElementName = "COMPONENTLISTNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "COMPONENTBASICQTY")]
    [MaxLength(50)]
    public string? BasicQuantity { get; set; }

    [XmlElement(ElementName = "MULTICOMPONENTITEMLIST.LIST")]
    public List<ComponentsItem> ComponentsItems { get; set; }
}


[XmlRoot(ElementName = "MULTICOMPONENTITEMLIST.LIST")]
public class ComponentsItem
{

    [XmlElement(ElementName = "NATUREOFITEM")]
    public ComponentType NatureOfItem { get; set; }

    [XmlElement(ElementName = "STOCKITEMNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? StockItem { get; set; }

    [XmlElement(ElementName = "GODOWNNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Godown { get; set; }

    //Percentage of Allocation in Voucher
    [XmlElement(ElementName = "ADDLCOSTALLOCPERC")]
    public string? CostAllocPercentage { get; set; }

    [XmlElement(ElementName = "ACTUALQTY")]
    public string? ActualQuantity { get; set; }
}

public enum ComponentType
{
    [XmlEnum(Name = "Component")]
    Component = 0,
    [XmlEnum(Name = "Scrap")]
    Scrap = 1,
    [XmlEnum(Name = "By-Product")]
    ByProduct = 2,
    [XmlEnum(Name = "Co-Product")]
    CoProduct = 3,
}

