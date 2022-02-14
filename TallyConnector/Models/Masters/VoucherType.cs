namespace TallyConnector.Models.Masters;

[XmlRoot(ElementName = "VOUCHERTYPE")]
[XmlType(AnonymousType = true)]
public class VoucherType : BasicTallyObject, ITallyObject
{
    public VoucherType()
    {
        LanguageNameList = new();
    }


    [XmlAttribute(AttributeName = "NAME")]
    [JsonIgnore]
    public string OldName { get; set; }

    private string name;

    [XmlElement(ElementName = "NAME")]
    [Required]
    public string Name
    {
        get
        {
            name = (name == null || name == string.Empty) ? OldName : name;
            return name;
        }
        set => name = value;
    }

    public string Alias { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    [XmlElement(ElementName = "PARENT")]
    public string Parent { get; set; }

    [XmlElement(ElementName = "NUMBERINGMETHOD")]
    public string NumberingMethod { get; set; }

    [XmlElement(ElementName = "USEZEROENTRIES")]
    public string UseZeroEntries { get; set; }

    [XmlElement(ElementName = "ISACTIVE")]
    public string IsActive { get; set; }

    [XmlElement(ElementName = "PRINTAFTERSAVE")]
    public string PrintAfterSave { get; set; }

    [XmlElement(ElementName = "USEFORPOSINVOICE")]
    public string UseforPOSInvoice { get; set; }

    [XmlElement(ElementName = "VCHPRINTBANKNAME")]
    public string VchPrintBankName { get; set; }

    [XmlElement(ElementName = "VCHPRINTTITLE")]
    public string VchPrintTitle { get; set; }

    [XmlElement(ElementName = "VCHPRINTJURISDICTION")]
    public string VchPrintJurisdiction { get; set; }

    [XmlElement(ElementName = "ISOPTIONAL")]
    public string IsOptional { get; set; }

    [XmlElement(ElementName = "COMMONNARRATION")]
    public string CommonNarration { get; set; }

    [XmlElement(ElementName = "MULTINARRATION")]
    public string MultiNarration { get; set; }  //Narration for each Ledger

    [XmlElement(ElementName = "ISDEFAULTALLOCENABLED")]
    public string IsDefaultAllocationEnabled { get; set; }

    [XmlElement(ElementName = "AFFECTSSTOCK")]
    public string EffectStock { get; set; }

    [XmlElement(ElementName = "ASMFGJRNL")]
    public string AsMfgJrnl { get; set; }

    [XmlElement(ElementName = "USEFORJOBWORK")]
    public string UseforJobwork { get; set; }

    [XmlElement(ElementName = "ISFORJOBWORKIN")]
    public string IsforJobworkIn { get; set; }


    [XmlElement(ElementName = "CANDELETE")]
    public string CanDelete { get; set; }


    /// <summary>
    /// Accepted Values //Create, Alter, Delete
    /// </summary>
    [JsonIgnore]
    [XmlAttribute(AttributeName = "Action")]
    public string Action { get; set; }

    public void CreateNamesList()
    {
        if (LanguageNameList.Count == 0)
        {
            LanguageNameList.Add(new LanguageNameList());
            LanguageNameList[0].NameList.NAMES.Add(Name);

        }
        if (Alias != null && Alias != string.Empty)
        {
            LanguageNameList[0].LanguageAlias = Alias;
        }
    }
    public new string GetXML(XmlAttributeOverrides attrOverrides = null)
    {
        CreateNamesList();
        return base.GetXML(attrOverrides);
    }
    public new void PrepareForExport()
    {
        CreateNamesList();
    }
}