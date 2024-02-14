﻿namespace TallyConnector.Core.Models.Masters.CostCenter;


[XmlRoot(ElementName = "COSTCATEGORY")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.CostCategories)]
public class CostCategory : BasicTallyObject, IAliasTallyObject
{
    public CostCategory()
    {
        LanguageNameList = new();
    }
    public CostCategory(string name)
    {
        LanguageNameList = new();
        Name = name;
    }

    [XmlAttribute(AttributeName = "NAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [JsonIgnore]
    public string? OldName { get; set; }

    private string? name;

    [XmlElement(ElementName = "NAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [Required]
    public string Name
    {
        get
        {
            name = name == null || name == string.Empty ? OldName : name;
            return name!;
        }
        set => name = value;
    }

    [XmlElement(ElementName = "ALLOCATEREVENUE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? AllocateRevenue { get; set; }

    [XmlElement(ElementName = "ALLOCATENONREVENUE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? AllocateNonRevenue { get; set; }

    [XmlIgnore]
    public string? Alias { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    public override string ToString()
    {
        return $"Cost Category - {Name}";
    }
}
