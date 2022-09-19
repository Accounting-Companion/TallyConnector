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
