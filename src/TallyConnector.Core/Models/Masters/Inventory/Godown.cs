namespace TallyConnector.Core.Models.Masters.Inventory;


[XmlRoot(ElementName = "GODOWN")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.Godowns)]
public class Godown : BaseMasterObject
{
    public Godown()
    {
        Addresses = new();
        LanguageNameList = new();
    }

    [XmlElement(ElementName = "OLDNAME")]
    [TDLField(Set = "$Name")]
    [JsonIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string OldName { get; set; }

   
    [XmlElement(ElementName = "PARENT")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = "PARENTID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    [TDLField(Set = "$GUID:Godown:$Parent")]
    public string? ParentId { get; set; }


    [XmlArray(ElementName = "ADDRESS.LIST")]
    [XmlArrayItem(ElementName = "ADDRESS")]
    [TDLCollection(CollectionName = "Address", ExplodeCondition = "$$NumItems:ADDRESS<1")]
    public List<string> Addresses { get; set; }


    [XmlElement(ElementName = "PINCODE")]
    public string? PinCode { get; set; }

    [XmlElement(ElementName = "PHONENUMBER")]
    public string? PhoneNumber { get; set; }

    [XmlElement(ElementName = "ISEXTERNAL")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsExternal { get; set; } // ThirdParty Stock with Us

    [XmlElement(ElementName = "ISINTERNAL")]
    public bool? IsInternal { get; set; } // Our Stock With Third Party

    [XmlElement(ElementName = "CANDELETE")]
    public bool? CanDelete { get; set; }


    [XmlIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [TDLField(Set = "$_FirstAlias")]
    public string? Alias { get; set; }


    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }




    public override string ToString()
    {
        return $"Godown - {Name}";
    }
}
