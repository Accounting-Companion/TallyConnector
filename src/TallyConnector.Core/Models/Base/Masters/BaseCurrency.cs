namespace TallyConnector.Core.Models.Base.Masters;

public class BaseCurrency : BaseMasterObject
{
    [XmlElement(ElementName = "ORIGINALNAME")]
    public new string Name { get; set; } = null!;

    [XmlElement(ElementName = "MAILINGNAME")]
    public string FormalName { get; set; } = null!;
}
