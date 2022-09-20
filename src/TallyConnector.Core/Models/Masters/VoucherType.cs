namespace TallyConnector.Core.Models.Masters;

[XmlRoot(ElementName = "VOUCHERTYPE")]
[XmlType(AnonymousType = true)]
public class VoucherType : BasicTallyObject, IAliasTallyObject
{
    public VoucherType()
    {
        LanguageNameList = new();
        Parent = string.Empty;
    }

    public VoucherType(string name, string parent)
    {
        LanguageNameList = new();
        Name = name;
        Parent = parent;

    }


    [XmlAttribute(AttributeName = "NAME")]
    [JsonIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? OldName { get; set; }

    private string? name;

    [XmlElement(ElementName = "NAME")]
    [Required]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string Name
    {
        get
        {
            name = name == null || name == string.Empty ? OldName : name;
            return name!;
        }
        set => name = value;
    }

    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Alias { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    [XmlElement(ElementName = "PARENT")]
    [Required]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string Parent { get; set; }

    [XmlElement(ElementName = "PARENTID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? ParentId { get; set; }

    [XmlElement(ElementName = "NUMBERINGMETHOD")]
    public string? NumberingMethod { get; set; }

    [XmlElement(ElementName = "USEZEROENTRIES")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? UseZeroEntries { get; set; }

    [XmlElement(ElementName = "ISACTIVE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsActive { get; set; }

    [XmlElement(ElementName = "PRINTAFTERSAVE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? PrintAfterSave { get; set; }

    [XmlElement(ElementName = "USEFORPOSINVOICE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? UseforPOSInvoice { get; set; }

    [XmlElement(ElementName = "VCHPRINTBANKNAME")]
    public string? VchPrintBankName { get; set; }

    [XmlElement(ElementName = "VCHPRINTTITLE")]
    public string? VchPrintTitle { get; set; }

    [XmlElement(ElementName = "VCHPRINTJURISDICTION")]
    public string? VchPrintJurisdiction { get; set; }

    [XmlElement(ElementName = "ISOPTIONAL")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsOptional { get; set; }

    [XmlElement(ElementName = "COMMONNARRATION")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? CommonNarration { get; set; }

    [XmlElement(ElementName = "MULTINARRATION")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? MultiNarration { get; set; }  //Narration for each Ledger

    [XmlElement(ElementName = "ISDEFAULTALLOCENABLED")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsDefaultAllocationEnabled { get; set; }

    [XmlElement(ElementName = "AFFECTSSTOCK")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? EffectStock { get; set; }

    [XmlElement(ElementName = "ASMFGJRNL")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? AsMfgJrnl { get; set; }

    [XmlElement(ElementName = "USEFORJOBWORK")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? UseforJobwork { get; set; }

    [XmlElement(ElementName = "ISFORJOBWORKIN")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsforJobworkIn { get; set; }


    [XmlElement(ElementName = "DEFAULTVOUCHERCATEGORY")]
    public DefaultVoucherCategory? DefaultVoucherCategory { get; set; }

    [XmlElement(ElementName = "COREVOUCHERTYPE")]
    public CoreVoucherType? CoreVoucherType { get; set; }


    [XmlElement(ElementName = "CANDELETE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? CanDelete { get; set; }


    public void CreateNamesList()
    {
        if (LanguageNameList.Count == 0)
        {
            LanguageNameList.Add(new LanguageNameList());
            LanguageNameList[0].NameList?.NAMES?.Add(Name);

        }
        if (Alias != null && Alias != string.Empty)
        {
            LanguageNameList[0].LanguageAlias = Alias;
        }
    }
    public new string GetXML(XmlAttributeOverrides? attrOverrides = null)
    {
        CreateNamesList();
        return base.GetXML(attrOverrides);
    }
    public new void PrepareForExport()
    {
        CreateNamesList();
    }


    public override string ToString()
    {
        return $"VoucherType - {Name}";
    }
}

public enum DefaultVoucherCategory
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "AccountingVch")]
    AccountingVch = 1,
    [XmlEnum(Name = "InventoryVch")]
    InventoryVch = 2,
    [XmlEnum(Name = "OrderVch")]
    OrderVch = 3,
    [XmlEnum(Name = "PayrollVch")]
    PayrollVch = 4,
    [XmlEnum(Name = "PayrollAttndVch")]
    PayrollAttndVch = 5,
}

public enum CoreVoucherType
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "Sales")]
    Sales = 1,
    [XmlEnum(Name = "Purchase")]
    Purchase = 2,
    [XmlEnum(Name = "DebitNote")]
    DebitNote = 3,
    [XmlEnum(Name = "CreditNote")]
    CreditNote = 4,
    [XmlEnum(Name = "Payment")]
    Payment = 5,
    [XmlEnum(Name = "Receipt")]
    Receipt = 6,
    [XmlEnum(Name = "Contra")]
    Contra = 7,
    [XmlEnum(Name = "Journal")]
    Journal = 8,
    [XmlEnum(Name = "SalesOrder")]
    SalesOrder = 9,
    [XmlEnum(Name = "PurchaseOrder")]
    PurchaseOrder = 10,
    [XmlEnum(Name = "Memo")]
    Memo = 11,
    [XmlEnum(Name = "Reversing Journal")]
    ReversingJournal = 12,
    [XmlEnum(Name = "MaterialIn")]
    MaterialIn = 13,
    [XmlEnum(Name = "MaterialOut")]
    MaterialOut = 14,
    [XmlEnum(Name = "JobWork In Order")]
    JobWorkInOrder = 15,
    [XmlEnum(Name = "JobWork Out Order")]
    JobWorkOutOrder = 16,
    [XmlEnum(Name = "ReceiptNote")]
    ReceiptNote = 17,
    [XmlEnum(Name = "DeliveryNote")]
    DeliveryNote = 18,
    [XmlEnum(Name = "PhysicalStock")]
    PhysicalStock = 19,
    [XmlEnum(Name = "Payroll")]
    Payroll = 20,
    [XmlEnum(Name = "Attendance")]
    Attendance = 21,
    [XmlEnum(Name = "RejectionsIn")]
    RejectionsIn = 22,
    [XmlEnum(Name = "RejectionsOut")]
    RejectionsOut = 23,
    [XmlEnum(Name = "StockJournal")]
    StockJournal = 24,
}