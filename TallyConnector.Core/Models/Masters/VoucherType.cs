namespace TallyConnector.Core.Models.Masters;

[XmlRoot(ElementName = "VOUCHERTYPE")]
[XmlType(AnonymousType = true)]
public class VoucherType : BasicTallyObject, ITallyObject
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
    public YesNo UseZeroEntries { get; set; }

    [XmlElement(ElementName = "ISACTIVE")]
    public YesNo IsActive { get; set; }

    [XmlElement(ElementName = "PRINTAFTERSAVE")]
    public YesNo PrintAfterSave { get; set; }

    [XmlElement(ElementName = "USEFORPOSINVOICE")]
    public YesNo UseforPOSInvoice { get; set; }

    [XmlElement(ElementName = "VCHPRINTBANKNAME")]
    public string? VchPrintBankName { get; set; }

    [XmlElement(ElementName = "VCHPRINTTITLE")]
    public string? VchPrintTitle { get; set; }

    [XmlElement(ElementName = "VCHPRINTJURISDICTION")]
    public string? VchPrintJurisdiction { get; set; }

    [XmlElement(ElementName = "ISOPTIONAL")]
    public YesNo IsOptional { get; set; }

    [XmlElement(ElementName = "COMMONNARRATION")]
    public YesNo CommonNarration { get; set; }

    [XmlElement(ElementName = "MULTINARRATION")]
    public YesNo MultiNarration { get; set; }  //Narration for each Ledger

    [XmlElement(ElementName = "ISDEFAULTALLOCENABLED")]
    public YesNo IsDefaultAllocationEnabled { get; set; }

    [XmlElement(ElementName = "AFFECTSSTOCK")]
    public YesNo EffectStock { get; set; }

    [XmlElement(ElementName = "ASMFGJRNL")]
    public YesNo AsMfgJrnl { get; set; }

    [XmlElement(ElementName = "USEFORJOBWORK")]
    public YesNo UseforJobwork { get; set; }

    [XmlElement(ElementName = "ISFORJOBWORKIN")]
    public YesNo IsforJobworkIn { get; set; }


    [XmlElement(ElementName = "CANDELETE")]
    public YesNo CanDelete { get; set; }


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