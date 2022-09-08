using System.Diagnostics;
using System.Text.RegularExpressions;

namespace TallyConnector.Services;
public class ConfigureServerPortHelper
{
    const string ServerPortPattern = "ServerPort=.*0+";
    const string ClientServerPattern = "Client Server=[a-zA-Z]+";

    /// <summary>
    /// Configures Tally to open odbc port on specified port
    /// default port will be 9000
    /// </summary>
    /// <param name="tallyProcessInfo">process information of tally</param>
    /// <param name="Port">Port on which tally we want tally to open port</param>
    /// <returns>true if sucess in restarting tally after changes</returns>
    public static bool ConfigureTallyServerPort(TallyProcessInfo tallyProcessInfo, int Port = 9000)
    {
        if (tallyProcessInfo is null)
        {
            return false;
        }

        string path = Path.Combine(tallyProcessInfo.RootFolder, "tally.ini");
        var Text = File.ReadAllText(path);

        Text = Regex.Replace(Text, ServerPortPattern, $"ServerPort={Port}");
        Text = Regex.Replace(Text, ClientServerPattern, "Client Server=Both");

        File.WriteAllText(path, Text);
        Process.GetProcessById(tallyProcessInfo.ProcessId).Kill();
        if (StartTally(tallyProcessInfo.ExePath))
        {
            return true;
        };
        return false;
    }

    /// <summary>
    /// starts any application provided in path
    /// </summary>
    /// <param name="Path">full path of application</param>
    /// <returns></returns>
    public static bool StartTally(string Path)
    {
        Process process = Process.Start(Path);
        if (process != null)
        {
            return true;
        }
        return false;
    }
}
