using Avalonia.Controls;
using AvaloniaDialogs.Views;

namespace VivaLaVCard.Views;

public partial class SettingsDialog : BaseDialog
{
    public SettingsDialog()
    {
        InitializeComponent();
        ThemeComboBox.SelectedIndex = (int)Settings.Instance.Theme;
        ThemeComboBox.SelectionChanged += ThemeComboBox_SelectionChanged;
    }

    private void ThemeComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        Settings.Instance.Theme = (Settings.ThemeEnum)ThemeComboBox.SelectedIndex;
        Settings.Instance.Apply();
        Settings.Instance.Save();
    }
}
