using System;
using System.Collections.Generic;
using System.Text;

namespace TC.TDLReportSourceGenerator.Models;
public class TDLFieldData
{
    public string? Set { get;  set; }
    public bool IncludeInFetch { get; set; } = false;
}
