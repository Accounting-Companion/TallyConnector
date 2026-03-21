namespace TallyConnector.Models;

public class InstalledTallyApp
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string InstallPath { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string UninstallString { get; set; } = string.Empty;
    
    // Status: "Running", "Not Running"
    public string Status { get; set; } = "Not Running";
    public DateTime? InstallDate { get; set; }
    public int? ProcessId { get; set; }
    public int? Port { get; set; }
}
