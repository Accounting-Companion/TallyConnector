namespace TallyConnector.Models.Base.Masters;

[TDLCollection(Type ="Currency")]
[GenerateMeta]
public partial class Currency : BaseMasterObject
{
    [XmlElement(ElementName = "ORIGINALNAME")]
    public new string Name { get; set; } = null!;

    [XmlElement(ElementName = "MAILINGNAME")]
    public string FormalName { get; set; } = null!;
}
