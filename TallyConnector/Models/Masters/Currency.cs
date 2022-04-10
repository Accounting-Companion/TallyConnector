namespace TallyConnector.Models.Masters;

[XmlRoot(ElementName = "CURRENCY")]
[XmlType(AnonymousType = true)]
public class Currency : BasicTallyObject, ITallyObject
{
    public Currency()
    {
        ExpandedSymbol = string.Empty;
        OriginalName = string.Empty;
    }

    /// <summary>
    /// Create New Currency - Currency("$","USD")
    /// </summary>
    /// <param name="originalName">Symbol For Currency</param>
    /// <param name="expandedSymbol">Expanded Symbol(In Words) for Currency</param>
    public Currency(string originalName, string expandedSymbol)
    {
        ExpandedSymbol = expandedSymbol;
        OriginalName = originalName;
    }


    [XmlElement(ElementName = "ORIGINALNAME")]
    [Column(TypeName = "nvarchar(5)")]
    public string OriginalName { get; set; }

    [XmlElement(ElementName = "EXPANDEDSYMBOL")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string ExpandedSymbol { get; set; }

    [XmlElement(ElementName = "MAILINGNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? MailingName { get; set; }


    [XmlElement(ElementName = "DECIMALSYMBOL")]
    [Column(TypeName = "nvarchar(10)")]
    public string? DecimalSymbol { get; set; }

    [XmlElement(ElementName = "DECIMALPLACES")]
    [MaxLength(1)]
    [Range(1, 4)]

    public int DecimalPlaces { get; set; }

    /// <summary>
    /// Tally Field - No.of Decimal Places for amount in Words
    /// </summary>
    [XmlElement(ElementName = "DECIMALPLACESFORPRINTING")]
    [MaxLength(1)]
    [Range(1, 4)]
    public int DecimalPlaces_Print { get; set; }

    [XmlElement(ElementName = "INMILLIONS")]
    [Column(TypeName = "nvarchar(3)")]
    public YesNo InMilllions { get; set; }

    [XmlElement(ElementName = "ISSUFFIX")]
    [Column(TypeName = "nvarchar(3)")]
    public YesNo IsSuffix { get; set; }

    [XmlElement(ElementName = "HASSPACE")]
    [Column(TypeName = "nvarchar(3)")]
    public YesNo HasSpace { get; set; }


    [JsonIgnore]
    [XmlAttribute(AttributeName = "Action")]
    public Action Action { get; set; }


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
        return $"{OriginalName} - {ExpandedSymbol}";
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

