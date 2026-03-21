using TallyConnector.Models;

namespace TallyConnector.Services.Helpers;

public interface ITallyDiscoveryService
{
    Task<List<TallyProcessInfo>> GetRunningTallyProcessesAsync();
    Task<List<InstalledTallyApp>> GetInstalledTallyApplicationsAsync();
}
