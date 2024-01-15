using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Models.TallyComplexObjects;
[TDLCollection(ExplodeCondition = "NOT $$IsEmpty:{0}")]
public class Rate : ITallyComplexObject, ITallyBaseObject
{
    [TDLField(TallyType= "Rate : Price")]
    public decimal BaseRate {  get; set; }

    [TDLField(TallyType = "Number", Set = "$$String:{0}:\"Forex\"", Invisible = "$$Value=#TC_Rate_BaseRate")]
    [Column(TypeName = "decimal(20,4)")]
    public decimal? ForexRate { get; set; }
    [TDLField(TallyType = "Rate : Unit Symbol")]
    public string Unit {  get; set; }
}
