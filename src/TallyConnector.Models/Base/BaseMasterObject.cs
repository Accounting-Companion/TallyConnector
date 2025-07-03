using TallyConnector.Models.Common;

namespace TallyConnector.Models.Base;


public partial class BaseMasterObject : TallyObject, IBaseMasterObject
{
    [XmlElement(ElementName = "NAME")]
    [Required]
    [Column(TypeName = $"nvarchar({MaxNameLength})")]
    public string Name { get; set; } = null!;
}

public partial class BaseAliasedMasterObject : BaseMasterObject

{
    [Column(TypeName = $"nvarchar({MaxNameLength})")]
    [TDLField(Set = "$_FirstAlias")]
    public string? Alias { get; set; }

    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; } = [];


    public override string ToString()
    {

        return Alias == null ? Name : $"{Name}, Alias - {Alias}";
    }
}