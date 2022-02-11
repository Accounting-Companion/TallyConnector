namespace TallyConnector.Models.Masters;

[XmlRoot(ElementName = "CURRENCY")]
[XmlType(AnonymousType = true)]
public class Currency : BasicTallyObject, ITallyObject
{
    [XmlElement(ElementName = "ORIGINALNAME")]
    public string OriginalName { get; set; }

    [XmlElement(ElementName = "MAILINGNAME")]
    public string MailingName { get; set; }

    [XmlElement(ElementName = "EXPANDEDSYMBOL")]
    public string ExpandedSymbol { get; set; }

    [XmlElement(ElementName = "DECIMALSYMBOL")]
    public string DecimalSymbol { get; set; }

    [XmlElement(ElementName = "DECIMALPLACES")]
    public int DecimalPlaces { get; set; }

    [XmlElement(ElementName = "INMILLIONS")]
    public string InMilllions { get; set; }

    [XmlElement(ElementName = "ISSUFFIX")]
    public string IsSuffix { get; set; }

    [XmlElement(ElementName = "HASSPACE")]
    public string HasSpace { get; set; }

    [XmlElement(ElementName = "DECIMALPLACESFORPRINTING")]
    public int DecimalPlaces_Print { get; set; }


    /// <summary>
    /// Accepted Values //Create, Alter, Delete
    /// </summary>
    [JsonIgnore]
    [XmlAttribute(AttributeName = "Action")]
    public string Action { get; set; }


    //[XmlElement(ElementName = "DAILYSTDRATES.LIST")]
    //public List<DailystdRate> DailystdRateList { get; set; }

    //[XmlElement(ElementName = "DAILYBUYINGRATES.LIST")]
    //public List<DailyBuyingRate> DailysBuyingRateList { get; set; }

    //[XmlElement(ElementName = "DAILYSELLINGRATES.LIST")]
    //public List<DailySellingRate> DailySellingRateList { get; set; }

    public new void PrepareForExport()
    {

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

