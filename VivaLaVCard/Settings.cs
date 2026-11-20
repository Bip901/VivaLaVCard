using System;
using System.IO;
using Avalonia;
using Avalonia.Styling;
using Configson;

namespace VivaLaVCard;

public class Settings
{
    private const string AppDataDirectoryName = "VivaLaVCard";
    private const string FileName = "settings.json";

    public static Settings Instance => _settings ??= Load();
    private static Settings? _settings;

    public enum ThemeEnum
    {
        FollowSystem = 0,
        Light,
        Dark,
    }

    public ThemeEnum Theme { get; set; } = ThemeEnum.FollowSystem;

    static Settings()
    {
        ConfigurationIO.SetBaseDirectory(AppDataDirectoryName, RelativePathType.RelativeToAppData);
    }

    private static Settings Load()
    {
        Settings result;
        try
        {
            result = ConfigurationIO.Load<Settings>(FileName);
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException)
        {
            result = GetDefault();
        }
        return result;
    }

    private static Settings GetDefault()
    {
        return new Settings();
    }

    public void Apply()
    {
        Application
            .Current
            ?.RequestedThemeVariant = Theme switch
            {
                ThemeEnum.Light => ThemeVariant.Light,
                ThemeEnum.Dark => ThemeVariant.Dark,
                _ => ThemeVariant.Default,
            };
    }

    public void Save()
    {
        ConfigurationIO.Save(this, FileName);
    }
}
