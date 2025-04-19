namespace TallyConnector.Core.Models.Base.Masters;

[TDLCollection(Type = "Group")]
[XmlRoot("GROUP")]
[XmlType(AnonymousType = true)]
public class BaseGroup : BaseAliasedMasterObject
{

    /// <summary>
    /// Name of parent group
    /// </summary>
    [XmlElement(ElementName = "PARENT")]
    public string? Parent { get; set; }


    /// <summary>
    /// Reserved Name of group, its set by tally and readonly
    /// </summary>
    [XmlElement(ElementName = "RESERVEDNAME")]
    [IgnoreForCreateDTO]
    public string? ReservedName { get; set; }

    /// <summary>
    ///  Nature of group field in UI <br/>
    ///  calculated based on <see cref="IsRevenue"/> and   <see cref="IsDeemedPositive"/>
    /// </summary>
    [XmlElement(ElementName = "ISREVENUE")]
    public bool IsRevenue { get; set; }

    /// <summary>
    /// <inheritdoc cref="IsRevenue"/>
    /// </summary>

    [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
    public bool IsDeemedPositive { get; set; }


    [XmlElement(ElementName = "AFFECTSGROSSPROFIT")]
    public bool AffectGrossProfit { get; set; }


    [XmlElement(ElementName = "ISSUBLEDGER")]
    public bool IsSubledger { get; set; }

    [XmlElement(ElementName = "SORTPOSITION")]
    [IgnoreForCreateDTO]
    public int SortPosition { get; set; }


    /// <summary>
    /// Tally Field - Method to Allocate when used in purchase invoice
    /// </summary>
    [XmlElement(ElementName = "ADDLALLOCTYPE")]
    [Column(TypeName = "nvarchar(25)")]
    public AdAllocType? AddlAllocType { get; set; }

    /// <summary>
    /// Tally Field - Used for Calculation
    /// </summary>
    [XmlElement(ElementName = "BASICGROUPISCALCULABLE")]
    public bool? IsCalculable { get; set; }

    /// <summary>
    /// Tally Field - Net Debit/Credit Balances for Reporting 
    /// </summary>
    [XmlElement(ElementName = "ISADDABLE")]
    public bool? IsAddable { get; set; }

    public override string ToString()
    {
        return $"Group - {base.ToString()}";
    }
}
