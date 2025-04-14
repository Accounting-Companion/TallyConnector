namespace TallyConnector.Core.Models.Common;

[XmlRoot(ElementName = "LANGUAGENAME.LIST")]
[TDLCollection(CollectionName = "LanguageName")]
public class LanguageNameList
{
    public LanguageNameList()
    {
        NameList = [];
    }

    [XmlArray(ElementName = "NAME.LIST")]
    [XmlArrayItem(ElementName = "NAME")]
    [TDLCollection(CollectionName = "Name")]
    public List<string> NameList { get; set; }

    [XmlElement(ElementName = "LANGUAGEID")]
    public int LanguageId { get; set; }
}

[XmlRoot(ElementName = "NAME.LIST")]

public class Names
{
    public Names()
    {
        NAMES = [];
    }

    [XmlElement(ElementName = "NAME")]
    [TDLCollection(CollectionName = "Name")]
    public List<string>? NAMES { get; set; }

    //[XmlAttribute(AttributeName = "TYPE")]
    //public string TYPE { get; set; }

    //[XmlText]
    //public string Text { get; set; }
}
