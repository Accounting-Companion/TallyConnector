using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Models;
public class ReportProgressHelper
{
    public ReportProgressHelper(int totalCount, int processedCount, int totalProcessedCount)
    {
        TotalCount = totalCount;
        ProcessedCount = processedCount;
        TotalProcessedCount = totalProcessedCount;
    }

    public int TotalCount { get; }
    public int ProcessedCount { get; }
    public int TotalProcessedCount { get; }
    public double Percentage => (double)TotalProcessedCount / TotalCount * 100;
}
