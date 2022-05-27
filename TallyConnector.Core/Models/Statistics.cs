namespace TallyConnector.Core.Models;


[XmlRoot(ElementName = "VOUCHERTYPE")]
[TDLCollection(CollectionName = "STATVchType")]
public class VoucherTypeStat : BaseStatistics
{

    [XmlElement(ElementName = "CANCELLEDCOUNT")]
    [TDLXMLSet(Set = "$CancVal")]
    public int CancelledCount { get; set; }

    public int TotalCount => CancelledCount + Count;

}

[XmlRoot(ElementName = "MASTERTYPE")]
[TDLCollection(CollectionName = "STATObjects")]
public class MasterTypeStat : BaseStatistics
{

}

public class BaseStatistics
{
    [XmlElement(ElementName = "NAME")]
    public string Name { get; set; }

    [XmlElement(ElementName = "COUNT")]
    [TDLXMLSet(Set = "$StatVal")]
    public int Count { get; set; }
}

[XmlRoot(ElementName = "STATISTICS")]
public class Statistics
{

    [XmlElement(ElementName = "VOUCHERTYPE")]
    [TDLCollection(CollectionName = "STATVchType")]
    public List<VoucherTypeStat>? VoucherTypeStats { get; set; }

    [XmlElement(ElementName = "MASTERTYPE")]
    [TDLCollection(CollectionName = "STATObjects")]
    public List<MasterTypeStat>? MasterStats { get; set; }

    [XmlIgnore]
    public int TotalVouchersCount { get; private set; }
    [XmlIgnore]
    public int TotalMastersCount { get; private set; }

    public void CalculateTotals()
    {
        VoucherTypeStats?.ForEach((VchStat) => TotalVouchersCount += VchStat.TotalCount);
        MasterStats?.ForEach((MastStat) => TotalMastersCount += MastStat.Count);
    }
}