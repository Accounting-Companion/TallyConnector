using TallyConnector.Core.Attributes.SourceGenerator;

namespace TallyConnector.Core.Models.Base.Masters;

//[ImplementTallyRequestableObject]
public partial class BaseCurrency : BaseMasterObject
{
    [XmlElement(ElementName = "ORIGINALNAME")]
    public new string Name { get; set; } = null!;

    [XmlElement(ElementName = "MAILINGNAME")]
    public string FormalName { get; set; } = null!;
}
