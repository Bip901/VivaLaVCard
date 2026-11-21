using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaDialogs.Views;
using VivaLaVCard.Resources;
using VivaLaVCard.Util;

namespace VivaLaVCard.Views;

public partial class SettingsDialog : BaseDialog
{
    private readonly ObservableCollection<string> startupPaths = [];

    public SettingsDialog()
    {
        InitializeComponent();
        StartupPathsListBox.ItemsSource = startupPaths;
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
        ThemeComboBox.SelectedIndex = (int)settings.Theme;
        MapsProviderComboBox.SelectedIndex = (int)settings.MapsProvider;
        UpdateMapsSearchUrlTextBox();
        startupPaths.Clear();
        if (settings.StartupPaths != null)
        {
            foreach (string path in settings.StartupPaths)
            {
                startupPaths.Add(path);
            }
        }
    }

    private void SaveSettings()
    {
        Settings settings = Settings.Instance;
        settings.Theme = (Settings.ThemeEnum)ThemeComboBox.SelectedIndex;
        MapsProvider mapsProvider = (MapsProvider)MapsProviderComboBox.SelectedIndex;
        settings.MapsProvider = mapsProvider;
        settings.CustomMapsSearchUrl = mapsProvider == MapsProvider.Custom ? MapsSearchUrlTextBox.Text : null;
        settings.StartupPaths = startupPaths.Count > 0 ? startupPaths.ToArray() : null;
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

    private async void AddStartupFileButton_Click(object? sender, RoutedEventArgs e)
    {
        IStorageProvider? storageProvider = TopLevel.GetTopLevel(this)?.StorageProvider;
        if (storageProvider is null)
        {
            return;
        }

        IReadOnlyList<IStorageFile> files = await storageProvider.OpenFilePickerAsync(
            UIUtil.CreateVCardFilePickerOpenOptions()
        );

        AddStartupPaths(files);
    }

    private async void AddStartupFolderButton_Click(object? sender, RoutedEventArgs e)
    {
        IStorageProvider? storageProvider = TopLevel.GetTopLevel(this)?.StorageProvider;
        if (storageProvider is null)
        {
            return;
        }

        IReadOnlyList<IStorageFolder> folders = await storageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions { Title = Strings.open_folder_title, AllowMultiple = true }
        );

        AddStartupPaths(folders);
    }

    private void RemoveStartupPathButton_Click(object? sender, RoutedEventArgs e)
    {
        if (StartupPathsListBox.SelectedItem is string path)
        {
            startupPaths.Remove(path);
        }
    }

    private void AddStartupPaths(IReadOnlyList<IStorageItem> items)
    {
        foreach (IStorageItem item in items)
        {
            string path = item.TryGetLocalPath() ?? item.Path.ToString();
            if (!startupPaths.Contains(path))
            {
                startupPaths.Add(path);
            }
        }
    }

    public override bool OnClosing()
    {
        SaveSettings();
        return base.OnClosing();
    }
}
