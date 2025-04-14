using System.Diagnostics;
using System.Net.NetworkInformation;

namespace TallyConnector.Services;
public static class GetTallyProcessesHelper
{
    public static List<TallyProcessInfo> GetTallyProcesses()
    {
        Process[] processes = Process.GetProcessesByName("tally");
        List<TallyProcessInfo> processInfos = [];
        foreach (Process process in processes)
        {
            TallyProcessInfo item = new (process);
            processInfos.Add(item);
        }
        return processInfos;
    }
}

[DebuggerDisplay("{ToString()}")]
public class TallyProcessInfo
{
    public TallyProcessInfo(Process process)
    {
        ProcessName = process.ProcessName;
        ProcessId = process.Id;
        ExePath = process.MainModule?.FileName ?? string.Empty;
        RootFolder = Path.GetDirectoryName(process.MainModule?.FileName ?? string.Empty) ?? string.Empty;

    }

    public string ProcessName { get; set; }
    public int ProcessId { get; set; }
    public string ExePath { get; set; }
    public string RootFolder { get; set; }

    public new string ToString()
    {
        return $"{RootFolder}";
    }
}
