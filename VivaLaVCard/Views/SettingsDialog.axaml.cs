using Avalonia.Controls;
using AvaloniaDialogs.Views;
using VivaLaVCard.Util;

namespace VivaLaVCard.Views;

public partial class SettingsDialog : BaseDialog
{
    public SettingsDialog()
    {
        InitializeComponent();
        LoadSettings();
        ThemeComboBox.SelectionChanged += ThemeComboBox_SelectionChanged;
        MapsProviderComboBox.SelectionChanged += (s, e) =>
        {
            UpdateMapsSearchUrlTextBox();
        };
    }

    private void LoadSettings()
    {
        Settings settings = Settings.Instance;
        ThemeComboBox.SelectedIndex = (int)Settings.Instance.Theme;
        MapsProviderComboBox.SelectedIndex = (int)settings.MapsProvider;
        UpdateMapsSearchUrlTextBox();
    }

    private void SaveSettings()
    {
        Settings settings = Settings.Instance;
        settings.Theme = (Settings.ThemeEnum)ThemeComboBox.SelectedIndex;
        MapsProvider mapsProvider = (MapsProvider)MapsProviderComboBox.SelectedIndex;
        settings.MapsProvider = mapsProvider;
        settings.CustomMapsSearchUrl = mapsProvider == MapsProvider.Custom ? MapsSearchUrlTextBox.Text : null;
        settings.Save();
    }

    private void UpdateMapsSearchUrlTextBox()
    {
        MapsProvider selectedMapsProvider = (MapsProvider)MapsProviderComboBox.SelectedIndex;
        if (selectedMapsProvider == MapsProvider.Custom)
        {
            MapsSearchUrlTextBox.IsEnabled = true;
            MapsSearchUrlTextBox.Text = string.Empty;
        }
        else
        {
            MapsSearchUrlTextBox.IsEnabled = false;
            MapsSearchUrlTextBox.Text = MapsUtil.GetSearchUrl(selectedMapsProvider);
        }
    }

    private void ThemeComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        Settings settings = Settings.Instance;
        settings.Theme = (Settings.ThemeEnum)ThemeComboBox.SelectedIndex;
        settings.Apply();
    }

    public override bool OnClosing()
    {
        SaveSettings();
        return base.OnClosing();
    }
}
