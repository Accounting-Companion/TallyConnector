using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Models.Common;
public class BaseStatistics : IBaseObject
{
    [XmlElement(ElementName = "COUNT")]
    [TDLField(Set = "if $$ISEMPTY:$StatVal then 0 else $StatVal")]
    public int Count { get; set; }
}

[TDLCollection(CollectionName = "STATObjects", Exclude = true)]
public class MasterStatistics : BaseStatistics
{
    [XmlElement(ElementName = "NAME")]
    public TallyObjectType Name { get; set; }

    public override string ToString()
    {
        return $"{Name} - {Count}";
    }
}

[TDLCollection(CollectionName = "STATVchType", Exclude = true)]
public class VoucherStatistics : BaseStatistics
{
    [XmlElement(ElementName = "NAME")]
    public string Name { get; set; }

    [XmlElement(ElementName = "CANCELLEDCOUNT")]
    [TDLField(Set = "if $$ISEMPTY:$CancVal then 0 else $CancVal")]
    public int CancelledCount { get; set; }

    [XmlElement(ElementName = "TOTALCOUNT")]
    [TDLField(Set = "if $$ISEMPTY:$MigVal then 0 else $MigVal")]
    public int TotalCount { get; set; }

    [XmlElement(ElementName = "OPTIONALCOUNT")]
    [TDLField(Set = "if $$ISEMPTY:$$DirectOptionalVch:$Name then 0 else $$DirectOptionalVch:$Name")]
    public int OptionalCount { get; set; }

    public int NetCount => Count - CancelledCount;

    public override string ToString()
    {
        return $"{Name} - {NetCount}";
    }
}