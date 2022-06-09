namespace TallyConnector.Core.Models.Masters;

[XmlRoot(ElementName = "CURRENCY")]
[XmlType(AnonymousType = true)]
public class Currency : BasicTallyObject, ITallyObject
{
    public Currency()
    {
        ExpandedSymbol = string.Empty;
        Name = string.Empty;
    }

    /// <summary>
    /// Create New Currency - Currency("$","USD")
    /// </summary>
    /// <param name="originalName">Symbol For Currency</param>
    /// <param name="expandedSymbol">Expanded Symbol(In Words) for Currency</param>
    public Currency(string originalName, string expandedSymbol)
    {
        ExpandedSymbol = expandedSymbol;
        Name = originalName;
    }


    [XmlElement(ElementName = "ORIGINALNAME")]
    [Required]
    [Column(TypeName = "nvarchar(5)")]
    public string Name { get; set; }

    [XmlElement(ElementName = "EXPANDEDSYMBOL")]
    [Required]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string ExpandedSymbol { get; set; }

    [XmlElement(ElementName = "MAILINGNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? MailingName { get; set; }


    [XmlElement(ElementName = "DECIMALSYMBOL")]
    [Column(TypeName = "nvarchar(10)")]
    public string? DecimalSymbol { get; set; }

    [XmlElement(ElementName = "DECIMALPLACES")]
    [Range(1, 4)]

    public int DecimalPlaces { get; set; }

    /// <summary>
    /// Tally Field - No.of Decimal Places for amount in Words
    /// </summary>
    [XmlElement(ElementName = "DECIMALPLACESFORPRINTING")]
    [Range(1, 4)]
    public int DecimalPlacesPrint { get; set; }

    [XmlElement(ElementName = "INMILLIONS")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? InMilllions { get; set; }

    [XmlElement(ElementName = "ISSUFFIX")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsSuffix { get; set; }

    [XmlElement(ElementName = "HASSPACE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? HasSpace { get; set; }



    //[XmlElement(ElementName = "DAILYSTDRATES.LIST")]
    //public List<DailystdRate> DailystdRateList { get; set; }

    //[XmlElement(ElementName = "DAILYBUYINGRATES.LIST")]
    //public List<DailyBuyingRate> DailysBuyingRateList { get; set; }

    //[XmlElement(ElementName = "DAILYSELLINGRATES.LIST")]
    //public List<DailySellingRate> DailySellingRateList { get; set; }

    public new void PrepareForExport()
    {

    }

    public override string ToString()
    {
        return $"{Name} - {ExpandedSymbol}";
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

