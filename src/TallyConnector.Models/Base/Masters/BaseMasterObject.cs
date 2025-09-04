using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using TallyConnector.Core.Models.DTO;
using TallyConnector.Models.Common;

namespace TallyConnector.Models.Base.Masters;


[MaptoDTO<BaseMasterObjectDTO>]
public partial class BaseMasterObject : TallyObject, IBaseMasterObject
{
    [XmlElement(ElementName = "NAME")]
    [Required]
    [Column(TypeName = $"nvarchar({MaxNameLength})")]
    public string Name { get; set; } = null!;

}
public class BaseMasterObjectDTO : TallyObjectDTO
{
    [XmlIgnore]
    public string NewName { get; set; } = null!;

    [XmlAttribute(AttributeName = "NAME")]
    public string Name { get; set; } = null!;
}
[MaptoDTO<BaseAliasedMasterObjectDTO>]
public partial class BaseAliasedMasterObject : BaseMasterObject

{
    [Column(TypeName = $"nvarchar({MaxNameLength})")]
    [TDLField(Set = "$_FirstAlias")]
    [IgnoreForCreateDTO]
    public string? Alias { get; set; }

    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; } = [];


    public override string ToString()
    {

        return Alias == null ? Name : $"{Name}, Alias - {Alias}";
    }
}
public class BaseAliasedMasterObjectDTO : BaseMasterObjectDTO
{
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<Common.DTO.LanguageNameListDTO> LanguageNameList { get; set; } = [];
    public void SetLanguageNameListAndAlias(string? alias = null)
    {
        LanguageNameList ??= [];
        if (LanguageNameList.Count == 0)
        {
            var names = new List<string> { NewName ?? string.Empty };
            if (alias is not null)
                names.Add(alias);
            LanguageNameList.Add(new()
            {
                Names = names
            });
            return;
        }
        var first = LanguageNameList[0];
        first.Names ??= [];
        if (first.Names.Count == 0)
            first.Names.Add(NewName);
        else
            first.Names[0] = NewName;

        if (alias is null)
        {
            return;
        }
        if (first.Names.Count == 1)
            first.Names.Add(alias);
        else
            first.Names[1] = alias;

    }
}