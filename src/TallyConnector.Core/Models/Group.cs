using TallyConnector.Core.Models.Common;

namespace TallyConnector.Core.Models;

public class BaseTallyGroup : TallyObject
{
    public BaseTallyGroup(string name)
    {
        Name = name;
    }
    public BaseTallyGroup(string name, string? ParentGroupName)
    {
        Name = name;
        Parent = ParentGroupName;
    }

    public override string CollectionObjectType => TallyConstants.OBJECTTYPES.Group.CollectionType;

    public override string XmlRootTag => TallyConstants.OBJECTTYPES.Group.XMLTAG;
    public string Name { get; set; }
    public string? Parent { get; set; }

    [TDLCollection(CollectionName = "LanguageName")]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    public List<LanguageNameList> LanguageNameList { get; set; }


}

public class LanguageNameList
{
    [XmlElement(ElementName = "NAME.LIST")]
    public Names NameList { get; set; }

    public int LanguageId { get; set; }
}
public class Names
{
    public List<string>? NAMES { get; set; }
}
public class Group : BaseTallyGroup
{
    public Group(string name) : base(name)
    {
    }

    public Group(string name, string? ParentGroupName) : base(name, ParentGroupName)
    {
    }
}
public class GroupCreate : IPostTallyObject
{
    public string Action { get; set; }
}



