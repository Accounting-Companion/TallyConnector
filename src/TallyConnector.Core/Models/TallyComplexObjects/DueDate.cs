using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Models.TallyComplexObjects;
[TDLCollection(ExplodeCondition = "NOT $$IsEmpty:{0}")]
public class DueDate : ITallyComplexObject, ITallyBaseObject
{
    [TDLField(Set = "$$string:{0}:UniversalDate")]
    public DateTime DueOnDate { get; set; }

    [TDLField(Set = "$$ExtractNumbers:$$DueDateInDays:{0}")]
    public int InDays { get; set; }

    [TDLField(Set = "{0}")]
    public string InText { get; set; }
}
