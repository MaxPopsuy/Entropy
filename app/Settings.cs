using Spectre.Console;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Entropy
{
    public enum InterfaceModeEnum
    {
        Generic,
        Classic
    }

    public enum ThemeEnum
    {
        Default,
        Experimental
    }

    public static class SettingsManager
    {
        private static string settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Entropy", "settings.json");

        public static void InitializeSettings()
        {
            if (!Directory.Exists(Path.GetDirectoryName(settingsFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath));
            }

            if (!File.Exists(settingsFilePath))
            {
                var defaultSettings = CreateDefaultSettings();
                SaveSettings(defaultSettings);
            }
        }

        public static Settings LoadSettings()
        {
            if (!File.Exists(settingsFilePath))
            {
                InitializeSettings();
            }

            Settings settings;
            try
            {
                string json = File.ReadAllText(settingsFilePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    AnsiConsole.MarkupLine($"[red]Settings file is empty. Initializing default settings.[/]");
                    settings = CreateDefaultSettings();
                    SaveSettings(settings);
                    return settings;
                }

                settings = JsonSerializer.Deserialize<Settings>(json);

                if (settings == null)
                {
                    AnsiConsole.MarkupLine($"[red]Settings file is corrupt. Initializing default settings.[/]");
                    settings = CreateDefaultSettings();
                    SaveSettings(settings);
                    return settings;
                }

                settings = ValidateSettings(settings);
                SaveSettings(settings);

                return settings;
            }
            catch (JsonException ex)
            {
                AnsiConsole.MarkupLine($"[red]Failed to deserialize settings: {ex.Message}. Initializing default settings.[/]");
                settings = CreateDefaultSettings();
                SaveSettings(settings);
                return settings;
            }
        }

        private static Settings CreateDefaultSettings()
        {
            var settings = new Settings();
            foreach (var property in typeof(Settings).GetProperties())
            {
                if (property.CanRead && property.CanWrite)
                {
                    var defaultValue = property.GetValue(new Settings());
                    property.SetValue(settings, defaultValue);
                }
            }

            return settings;
        }

        public static void SaveSettings(Settings settings)
        {
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(settingsFilePath, json);
        }
        public static object GetSettingValue(Settings settings, string propertyName)
        {
            var propertyInfo = typeof(Settings).GetProperty(propertyName);
            Console.WriteLine(propertyInfo.GetValue(propertyName).ToString());
            return propertyInfo != null ? propertyInfo.GetValue(settings) : null;
        }

        private static Settings ValidateSettings(Settings settings)
        {
            foreach (var property in typeof(Settings).GetProperties())
            {
                if (property.PropertyType.IsEnum)
                {
                    var currentValue = property.GetValue(settings);

                    if (!Enum.IsDefined(property.PropertyType, currentValue))
                    {

                        var defaultEnumValue = Enum.GetValues(property.PropertyType).GetValue(0);
                        property.SetValue(settings, defaultEnumValue);
                    }
                }
            }
            return settings;
        }
    }

    [JsonSerializable(typeof(Settings))]
    public class Settings
    {
        // Generic
        public bool AutoStart { get; set; } = false;
        public bool CheckForUpdates { get; set; } = true;
        public bool PHAutoUpdate { get; set; } = false;

        // Functions
        // EXPERIMENTAL
        public bool PHExperimentalProcessListAutoRefresh { get; set; } = false;
        public int PHExperimentalProcessListAutoRefreshRate { get; set; } = 500;
        public bool PHExperimentalShowGpuCpuUsage { get; set; } = false;
        public string PHExperimentalLanguage { get; set; } = "en";

        public ThemeEnum PHExperimentalTheme { get; set; } = ThemeEnum.Default;

        // UI
        public InterfaceModeEnum PHInterfaceMode { get; set; } = InterfaceModeEnum.Generic;
    }
    public static class SettingsActions
    {
        public static void ExperimentalTheme()
        {
            AnsiConsole.MarkupLine("[green]Checking for updates...[/]");
            Task.Delay(1000).Wait();
            AnsiConsole.MarkupLine("[green]Update check completed![/]");
        }

        public static void HandleCheckForUpdates(bool value)
        {
            AnsiConsole.MarkupLine($"[green]CheckForUpdates and related settings updated to '{value}'.[/]");
        }
        public static void HandleAutoUpdate(bool value)
        {
            throw new NotImplementedException();
        }
        public static void HandleAutoStart(bool enable)
        {
            if (enable)
            {
                AutostartManager.EnableAutoStart();
            }
            else
            {
                AutostartManager.DisableAutoStart();
            }
        }
    }
}
