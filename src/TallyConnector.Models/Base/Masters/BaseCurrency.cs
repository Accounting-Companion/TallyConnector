namespace TallyConnector.Models.Base.Masters;

[ImplementTallyRequestableObject]
[TDLCollection(Type ="Currency")]
public partial class BaseCurrency : BaseMasterObject
{
    [XmlElement(ElementName = "ORIGINALNAME")]
    public new string Name { get; set; } = null!;

    [XmlElement(ElementName = "MAILINGNAME")]
    public string FormalName { get; set; } = null!;
}
