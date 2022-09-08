using System.Diagnostics;

namespace TallyConnector.Services;
public static class GetTallyProcessesHelper
{
    public static List<TallyProcessInfo> GetTallyProcesses()
    {
        Process[] processes = Process.GetProcessesByName("tally");
        List<TallyProcessInfo> processInfos = new();
        foreach (Process process in processes)
        {
            processInfos.Add(new TallyProcessInfo(process));
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
        ExePath = process.MainModule.FileName;
        RootFolder = Path.GetDirectoryName(process.MainModule.FileName);
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
