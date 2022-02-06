namespace TallyConnector.Models;

public class BaseTallyClass : TallyXmlJson
{
    [XmlAttribute(AttributeName = "ID")]
    public int TallyId { get; set; }


}

