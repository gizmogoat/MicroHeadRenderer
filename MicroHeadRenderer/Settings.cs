using Newtonsoft.Json;

namespace MicroHeadRenderer;

public class Settings
{
    public required string SessionServer { get; set; }
    public required int Port { get; set; }
}

public class SettingsHelper
{
    private static string SettingsPath;
    public Settings Settings;
    public SettingsHelper()
    {
        if (!Path.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MicroHeadRenderer")))
        {
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MicroHeadRenderer"));
        }
        SettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MicroHeadRenderer/settings.json");
        if (!File.Exists(SettingsPath))
        {
            var exampleConfig = new Settings() { SessionServer = "https://drasl.unmojang.org", Port = /* literally */ 1984 };
            File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(exampleConfig));
            Console.WriteLine("Example server config created. Please set your real values in the settings file and restart MicroHeadRenderer");
        } else 
            Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsPath));
    }
}