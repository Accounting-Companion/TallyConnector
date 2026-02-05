namespace TallyConnector.Models.Common;


//Related to payroll and ledger
[XmlRoot(ElementName = "PAYMENTDETAILS.LIST")]
[GenerateMeta]
public partial class PaymentDetails
{
    [XmlElement(ElementName = "BANKNAME")]
    public string BankName { get; set; } = string.Empty;

    [XmlElement(ElementName = "CITY")]
    public string? City { get; set; }

    [XmlElement(ElementName = "DEFAULTTRANSACTIONTYPE")]
    public string? DefaultTransactionType { get; set; }

    [XmlElement(ElementName = "PAYMENTFAVOURING")]
    public string? InFavour { get; set; }

    [XmlElement(ElementName = "TRANSACTIONNAME")]
    public string? TransactionName { get; set; }

    [XmlElement(ElementName = "CHEQUECROSSCOMMENT")]
    public string? ChequeCrossComment { get; set; }

    [XmlElement(ElementName = "SETASDEFAULT")]
    public bool SetasDefault { get; set; } = false;

    [XmlElement(ElementName = "ACCOUNTNUMBER")]
    public string? BankAccountNo { get; set; }

    [XmlElement(ElementName = "BANKBRANCH")]
    public string? BankBranch { get; set; }

    [XmlElement(ElementName = "IFSCODE")]
    public string? IFSC { get; set; }

}
