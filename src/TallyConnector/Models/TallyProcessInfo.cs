namespace TallyConnector.Models;

public class TallyProcessInfo
{
    public int ProcessId { get; set; }
    public string ProcessName { get; set; } = string.Empty;
    public string MainWindowTitle { get; set; } = string.Empty;
    public string? Path { get; set; }
    public bool IsAccessible { get; set; }
    public bool IsResponding { get; set; }
}
