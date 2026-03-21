using System.Diagnostics;
using Microsoft.Win32;
using TallyConnector.Models;

namespace TallyConnector.Services.Helpers;

public class TallyDiscoveryService : ITallyDiscoveryService
{
    public Task<List<TallyProcessInfo>> GetRunningTallyProcessesAsync()
    {
        return Task.Run(() =>
        {
            var tallyProcesses = new List<TallyProcessInfo>();
            var processes = Process.GetProcessesByName("tally");

            foreach (var process in processes)
            {
                string? path = null;
                bool isAccessible = false;
                try
                {
                    // Attempt to get the path. This might fail if we don't have permissions (e.g. process running as admin)
                    path = process.MainModule?.FileName;
                    isAccessible = true;
                }
                catch
                {
                    // Ignore permission errors, path remains null, isAccessible remains false
                }

                tallyProcesses.Add(new TallyProcessInfo
                {
                    ProcessId = process.Id,
                    ProcessName = process.ProcessName,
                    MainWindowTitle = process.MainWindowTitle,
                    Path = path,
                    IsAccessible = isAccessible,
                    IsResponding = process.Responding
                });
            }

            return tallyProcesses;
        });
    }

    public async Task<List<InstalledTallyApp>> GetInstalledTallyApplicationsAsync()
    {
        // Get running processes first to populate status
        var runningProcesses = await GetRunningTallyProcessesAsync();

        return await Task.Run(() =>
        {
            var installedApps = new List<InstalledTallyApp>();
            var registryViews = new[] { RegistryView.Registry32, RegistryView.Registry64 };
            var registryHives = new[] { RegistryHive.CurrentUser, RegistryHive.LocalMachine };

            foreach (var hive in registryHives)
            {
                foreach (var view in registryViews)
                {
                    try
                    {
                        using var baseKey = RegistryKey.OpenBaseKey(hive, view);
                        using var uninstallKey = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");

                        if (uninstallKey != null)
                        {
                            foreach (var subKeyName in uninstallKey.GetSubKeyNames())
                            {
                                using var subKey = uninstallKey.OpenSubKey(subKeyName);
                                if (subKey != null)
                                {
                                    var displayName = subKey.GetValue("DisplayName") as string;
                                    var publisher = subKey.GetValue("Publisher") as string;

                                    if ((!string.IsNullOrWhiteSpace(displayName) && displayName.Contains("TallyPrime", StringComparison.OrdinalIgnoreCase)) ||
                                        (!string.IsNullOrWhiteSpace(publisher) && publisher.Contains("Tally Solutions", StringComparison.OrdinalIgnoreCase)))
                                    {
                                        var installLocation = subKey.GetValue("InstallLocation") as string ?? string.Empty;
                                        
                                        // Determine status
                                        string status = "Not Running";
                                        int? processId = null;
                                        int? port = null;

                                        if (!string.IsNullOrWhiteSpace(installLocation))
                                        {
                                            // Normalize path for comparison (trim trailing slash)
                                            var normalizedInstallPath = installLocation.TrimEnd('\\', '/');
                                            
                                            // Find matching process
                                            var matchingProcess = runningProcesses.FirstOrDefault(p => 
                                                p.Path != null && 
                                                (System.IO.Path.GetDirectoryName(p.Path)?.Equals(normalizedInstallPath, StringComparison.OrdinalIgnoreCase) ?? false));

                                            if (matchingProcess != null)
                                            {
                                                status = "Running";
                                                processId = matchingProcess.ProcessId;
                                                port = GetPortForProcess(processId.Value);
                                            }
                                        }

                                        var installDateStr = subKey.GetValue("InstallDate") as string;
                                        DateTime? installDate = null;
                                        if (!string.IsNullOrEmpty(installDateStr) && 
                                            System.DateTime.TryParseExact(installDateStr, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var date))
                                        {
                                            installDate = date;
                                        }

                                        installedApps.Add(new InstalledTallyApp
                                        {
                                            Name = displayName ?? string.Empty,
                                            Version = subKey.GetValue("DisplayVersion") as string ?? string.Empty,
                                            InstallPath = installLocation,
                                            Publisher = publisher ?? string.Empty,
                                            UninstallString = subKey.GetValue("UninstallString") as string ?? string.Empty,
                                            Status = status,
                                            ProcessId = processId,
                                            InstallDate = installDate,
                                            Port = port
                                        });
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Ignore errors accessing specific keys
                    }
                }
            }

            // Sort by InstallDate Descending (Latest First)
            return installedApps.OrderByDescending(x => x.InstallDate).ToList();
        });
    }

    private int? GetPortForProcess(int processId)
    {
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "netstat",
                Arguments = "-ano",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            if (process == null) return null;

            using var reader = process.StandardOutput;
            string output = reader.ReadToEnd();
            process.WaitForExit();

            var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.Contains(processId.ToString()))
                {
                    // Line format: Proto Local Address Foreign Address State PID
                    // Example: TCP 0.0.0.0:9000 0.0.0.0:0 LISTENING 1234
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 5 && parts[4] == processId.ToString() && parts[3] == "LISTENING" && parts[0].StartsWith("TCP"))
                    {
                        var localAddress = parts[1];
                        var portPart = localAddress.Split(':').LastOrDefault();
                        if (int.TryParse(portPart, out int port))
                        {
                            return port;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting port for process {processId}: {ex.Message}");
        }
        return null;
    }
}
