namespace TallyConnector.Core.Models.TallyPrime3;
[TDLCollection(Type = "Voucher")]
public class Prime3Voucher : Voucher
{
    [XmlElement(ElementName = "GSTREGISTRATION")]
    public GSTRegistration? GSTRegistration { get; set; }

   
}