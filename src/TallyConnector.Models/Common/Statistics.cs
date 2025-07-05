using TallyConnector.Core.Models;

namespace TallyConnector.Models.Common;
public partial class BaseStatistics : IBaseObject
{
    [XmlElement(ElementName = "COUNT")]
    [TDLField(Set = "if $$ISEMPTY:$StatVal then 0 else $StatVal")]
    public int Count { get; set; }
}

[TDLCollection(CollectionName = "STATObjects", Exclude = true)]
[GenerateITallyRequestableObect]
[GenerateMeta]
public partial class MasterStatistics : BaseStatistics
{
    [XmlElement(ElementName = "NAME")]
    public TallyObjectType Name { get; set; }

    public override string ToString()
    {
        return $"{Name} - {Count}";
    }
}

[TDLCollection(CollectionName = "STATVchType", Exclude = true)]
[GenerateITallyRequestableObect]
[GenerateMeta]
public partial class VoucherStatistics : BaseStatistics
{
    [XmlElement(ElementName = "NAME")]
    public string Name { get; set; } = null!;

    [XmlElement(ElementName = "CANCELLEDCOUNT")]
    [TDLField(Set = "if $$ISEMPTY:$CancVal then 0 else $CancVal")]
    public int CancelledCount { get; set; }

    [XmlElement(ElementName = "TOTALCOUNT")]
    [TDLField(Set = "if $$ISEMPTY:$MigVal then 0 else $MigVal")]
    public int TotalCount { get; set; }

    [XmlElement(ElementName = "OPTIONALCOUNT")]
    [TDLField(Set = "if $$ISEMPTY:$$DirectOptionalVch:$Name then 0 else $$DirectOptionalVch:$Name")]
    public int OptionalCount { get; set; }


    public override string ToString()
    {
        return $"{Name} - {Count}";
    }
}