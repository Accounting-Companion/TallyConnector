﻿namespace TallyConnector.Models.Common;

[XmlRoot(ElementName = "LANGUAGENAME.LIST")]
[TDLCollection(CollectionName = "LanguageName")]
[GenerateMeta]
public partial class LanguageNameList
{
    public LanguageNameList()
    {
        Names = [];
    }

    [XmlArray(ElementName = "NAME.LIST")]
    [XmlArrayItem(ElementName = "NAME")]
    [TDLCollection(CollectionName = "Name")]
    public List<string> Names { get; set; }

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
