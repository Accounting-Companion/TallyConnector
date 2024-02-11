﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TallyConnector.TDLReportSourceGenerator.Models;
public class TDLFieldData
{
    public string? Set { get; set; }
    public bool IncludeInFetch { get; set; } = false;
    public string? Use { get; internal set; }
    public string? TallyType { get; internal set; }
    public string? Format { get; internal set; }
    public string? Invisible { get; internal set; }
    public string? FetchText { get; internal set; }
}
