using TallyConnector.Attributes;

namespace TallyConnector.Models;

[XmlRoot(ElementName = "LANGUAGENAME.LIST")]
public class LanguageNameList
{
    public LanguageNameList()
    {
        NameList = new();

    }
    [JsonIgnore]
    [XmlIgnore]
    public string LanguageAlias
    {
        get { return NameList.NAMES.Count > 1 ? string.Join("..\n", NameList.NAMES.GetRange(1, NameList.NAMES.Count - 1)) : null; }
        set
        {
            if (NameList.NAMES.Count > 1)
            {
                NameList.NAMES.RemoveRange(1, NameList.NAMES.Count - 1);
                NameList.NAMES.InsertRange(1, value.Split("..\n".ToCharArray()).ToList());
            }
            else if (NameList.NAMES.Count == 1)
            {
                NameList.NAMES.InsertRange(1, value.Split("..\n".ToCharArray()).ToList());
            }
        }
    }
    
    [XmlElement(ElementName = "NAME.LIST")]    
    public Names NameList { get; set; }

    //[XmlElement(ElementName = "LANGUAGEID")]
    //public LANGUAGEID LANGUAGEID { get; set; }
}

[XmlRoot(ElementName = "NAME.LIST")]

public class Names
{
    public Names()
    {
        NAMES = new();
    }

    [XmlElement(ElementName = "NAME")]
    [TDLCollection(CollectionName = "Name")]
    public List<string> NAMES { get; set; }

    //[XmlAttribute(AttributeName = "TYPE")]
    //public string TYPE { get; set; }

    //[XmlText]
    //public string Text { get; set; }
}
