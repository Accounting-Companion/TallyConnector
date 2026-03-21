using System.Text.Json;
using System.IO;

namespace DemoDesktopApp.Models;

public class AppSettings
{
    public int TallyPort { get; set; } = 9000;
    public string TallyBaseUrl { get; set; } = "http://localhost";

    private static string SettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DemoDesktopApp", "settings.json");

    public static AppSettings Load()
    {
        if (File.Exists(SettingsPath))
        {
            try
            {
                var json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch
            {
                // Ignore load errors and return default
            }
        }
        return new AppSettings();
    }

    public void Save()
    {
        var dir = Path.GetDirectoryName(SettingsPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir!);
        }
        var json = JsonSerializer.Serialize(this);
        File.WriteAllText(SettingsPath, json);
    }
}
