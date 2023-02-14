using System.Xml.Linq;
using TallyConnector.Core.Attributes;

namespace TallyConnector.Core.Models.Interfaces.Masters.Group;
public interface ICreateBaseGroup : ICreateNameAndAliasTallyObject
{
    string? Parent { get; set; }
}
public interface IReadBaseGroup : IReadNameAndAliasTallyObject
{
    string? Parent { get; set; }
}
public interface IBaseGroup : INameAndAliasTallyObject
{
    string? Parent { get; set; }
}

public interface IGroup : IBaseGroup
{

}
public class TallyObject : ITallyObject
{
    [XmlAttribute(AttributeName = "Action")]
    public Action Action { get; set; }
    public static  List<string> FetchList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void PrepareForExport()
    {
        throw new NotImplementedException();
    }
}
public partial class NamedTallyObject : TallyObject, INamedTallyObject
{
    [XmlElement(ElementName = "NAME")]
    public string Name { get; set; }
}
public class NameandAliasTallyObject : NamedTallyObject, INameAndAliasTallyObject
{
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList>? LanguageNameList { get; set; }
}
[XmlRoot("GROUP")]
public partial class BaseGroup : NameandAliasTallyObject, IBaseGroup
{

    [XmlElement(ElementName = "PARENT")]
    public string? Parent { get; set; }

    public new static List<string> FetchList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

}
[XmlRoot("GROUP")]
public class Group : BaseGroup, IGroup
{

}

//public class CreateBaseGroup : ICreateBaseGroup
//{

//}
public class ReadBaseGroup : IReadBaseGroup
{
    public string Name { get; set; }
    public string? Alias { get; set; }
    public string? Parent { get; set; }

    public int MasterId { get; set; }

    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? GUID { get; set; }

    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? RemoteId { get; set; }

    public int AlterId { get; set; }


}
