namespace TallyConnector.Models.Common;

public class AutoColVoucherTypeStat : IBaseObject
{
    [XmlElement(ElementName = "NAME")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// returns total count irrespective of period
    /// </summary>

    [XmlElement(ElementName = "TOTALCOUNT")]
    public ulong TotalCount { get; set; }

    /// <summary>
    /// rerurns count based on period
    /// </summary>
    [XmlElement(ElementName = "COUNT")]
    public ulong Count { get; set; }

    [XmlElement(ElementName = "CANCELLEDCOUNT")]
    public ulong CancelledCount { get; set; }

    [XmlElement(ElementName = "OTIONALCOUNT")]
    public ulong OptionalCount { get; set; }

    [XmlElement(ElementName = "PERIODSTAT")]
    public List<PeriodStat> PeriodStats { get; set; } = [];
}

public class PeriodStat
{
    [XmlElement(ElementName = "FROMDATE")]
    public DateTime FromDate { get; set; }

    [XmlElement(ElementName = "TODATE")]
    public DateTime ToDate { get; set; }

    [XmlElement(ElementName = "CANCELLEDCOUNT")]
    public ulong CancelledCount { get; set; }

    [XmlElement(ElementName = "OTIONALCOUNT")]
    public ulong OptionalCount { get; set; }

    [XmlElement(ElementName = "TOTALCOUNT")]
    public ulong TotalCount { get; set; }

    public override string ToString()
    {
        return $"{FromDate} - {ToDate} ,TotalCount - {TotalCount},CancCount - {CancelledCount},OptCount - {OptionalCount}";
    }
}

[XmlRoot(ElementName = "ENVELOPE")]
public class AutoVoucherStatisticsEnvelope
{
    [XmlElement(ElementName = "VCHTYPESTAT")]
    public List<AutoColVoucherTypeStat>? VoucherTypeStats { get; set; }
}
