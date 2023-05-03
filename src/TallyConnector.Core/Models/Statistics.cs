namespace TallyConnector.Core.Models;


[XmlRoot(ElementName = "VOUCHERTYPE")]
[TDLCollection(CollectionName = "STATVchType", Include = false)]
public class VoucherTypeStat : BaseStatistics, IReportInterfaceGenerator<MasterTypeStat>
{
    [XmlElement(ElementName = "CANCELLEDCOUNT")]
    [TDLXMLSet(Set = "if $$ISEMPTY:$CancVal then 0 else $CancVal")]
    public int CancelledCount { get; set; }

    public int NetCount => Count - CancelledCount;

    public IEnumerable<string> GetFields()
    {
        throw new NotImplementedException();
    }

    public string GetXMLTDLReport()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {

        return $"{Name} - {NetCount}(C-{CancelledCount}) ";
    }

}

[XmlRoot(ElementName = "MASTERTYPE")]
[TDLCollection(CollectionName = "STATObjects")]
public class MasterTypeStat : BaseStatistics, IReportInterfaceGenerator
{
    public IEnumerable<string> GetFields()
    {
        throw new NotImplementedException();
    }

    public string GetXMLTDLReport()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {

        return $"{Name} - {Count}) ";
    }
}

public class BaseStatistics
{
    [XmlElement(ElementName = "NAME")]
    public string Name { get; set; }

    [XmlElement(ElementName = "COUNT")]
    [TDLXMLSet(Set = "if $$ISEMPTY:$StatVal then 0 else $StatVal")]
    public int Count { get; set; }
}


[XmlRoot(ElementName = "VOUCHERTYPESTAT.LIST")]
public class VoucherStatistics : TallyBaseObject
{
    [XmlElement(ElementName = "VOUCHERTYPESTAT")]
    public List<VoucherTypeStat>? VoucherStats { get; set; }
}

[XmlRoot(ElementName = "MASTERTYPESTAT.LIST")]
public class MasterStatistics : TallyBaseObject
{
    [XmlElement(ElementName = "MASTERTYPESTAT")]
    public List<MasterTypeStat>? MasterStats { get; set; }
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
        VoucherTypeStats?.ForEach((VchStat) => TotalVouchersCount += VchStat.NetCount);
        MasterStats?.ForEach((MastStat) => TotalMastersCount += MastStat.Count);
    }
}