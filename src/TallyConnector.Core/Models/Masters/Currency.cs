using TallyConnector.Core.Models.Interfaces.Masters;

namespace TallyConnector.Core.Models.Masters;


[XmlRoot(ElementName = "CURRENCY")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.Currencies)]
public class BaseCurrency : TallyObject, IBaseCurrency
{
    public BaseCurrency()
    {
        FormalName = string.Empty;
        Name = string.Empty;
    }

    /// <summary>
    /// Create New Currency - Currency("$","USD")
    /// </summary>
    /// <param name="originalName">Symbol For Currency</param>
    /// <param name="formalName">Expanded Symbol(In Words) for Currency</param>
    public BaseCurrency(string originalName, string formalName)
    {
        Name = originalName;
        FormalName = formalName;
    }

    [XmlElement(ElementName = "ORIGINALNAME")]
    [Required]
    [Column(TypeName = "nvarchar(5)")]
    public string Name { get; set; }

    [XmlElement(ElementName = "MAILINGNAME")]
    [Required]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string FormalName { get; set; }


}


[XmlRoot(ElementName = "CURRENCY")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.Currencies)]
public class Currency : BaseCurrency
{

    /// <inheritdoc/>
    public Currency() : base()
    {
    }
    /// <inheritdoc/>
    public Currency(string originalName, string formalName) : base(originalName, formalName)
    {
    }

    [XmlElement(ElementName = "OLDNAME")]
    [TDLField(Set = "$Name")]
    [JsonIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string OldName { get; set; }


    [XmlElement(ElementName = "DECIMALSYMBOL")]
    [Column(TypeName = "nvarchar(10)")]
    public string? DecimalSymbol { get; set; }

    [XmlElement(ElementName = "DECIMALPLACES")]
    [Range(0, 4)]
    public int DecimalPlaces { get; set; }

    /// <summary>
    /// Tally Field - No.of Decimal Places for amount in Words
    /// </summary>
    [XmlElement(ElementName = "DECIMALPLACESFORPRINTING")]
    [Range(0, 4)]
    public int DecimalPlacesPrint { get; set; }

    [XmlElement(ElementName = "TC_INMILLIONS")]
    [TDLField(Set = "$InMilllions")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? InMilllions { get; set; }

    [XmlElement(ElementName = "ISSUFFIX")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsSuffix { get; set; }

    [XmlElement(ElementName = "HASSPACE")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? HasSpace { get; set; }



    //[XmlElement(ElementName = "DAILYSTDRATES.LIST")]
    //public List<DailystdRate> DailystdRateList { get; set; }

    //[XmlElement(ElementName = "DAILYBUYINGRATES.LIST")]
    //public List<DailyBuyingRate> DailysBuyingRateList { get; set; }

    //[XmlElement(ElementName = "DAILYSELLINGRATES.LIST")]
    //public List<DailySellingRate> DailySellingRateList { get; set; }


    public override string ToString()
    {
        return $"Currency {Name} - {FormalName}";
    }
}
//[XmlRoot(ElementName = "DAILYSTDRATES.LIST")]
//public class DailystdRate
//{

//    [XmlElement(ElementName = "DATE")]
//    public int Date { get; set; }

//    [XmlElement(ElementName = "SPECIFIEDRATE")]
//    public string SpecifiedRate { get; set; }
//}

//[XmlRoot(ElementName = "DAILYSELLINGRATES.LIST")]
//public class DailySellingRate : DailystdRate
//{
//}
//[XmlRoot(ElementName = "DAILYBUYINGRATES.LIST")]
//public class DailyBuyingRate : DailystdRate
//{
//}
