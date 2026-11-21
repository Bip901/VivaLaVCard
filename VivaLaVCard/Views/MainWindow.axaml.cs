using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using VivaLaVCard.Resources;
using VivaLaVCard.Util;
using VivaLaVCard.ViewModels;

namespace VivaLaVCard.Views;

public partial class MainWindow : Window
{
    public MainWindowViewModel ViewModel { get; }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = ViewModel = new MainWindowViewModel();

        AddHandler(DragDrop.DragEnterEvent, OnDragOver, handledEventsToo: true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver, handledEventsToo: true);
        AddHandler(DragDrop.DropEvent, OnDrop, handledEventsToo: true);
        AddHandler(KeyDownEvent, OnWindowKeyDown, RoutingStrategies.Tunnel);
        SearchBox.AddHandler(KeyDownEvent, OnSearchBoxKeyDown, RoutingStrategies.Tunnel);
    }

    private static void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.TryGetFiles()?.Length > 0 ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        e.Handled = true;

        IStorageItem[]? items = e.DataTransfer.TryGetFiles();
        if (items is null || items.Length == 0)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }
        e.DragEffects = DragDropEffects.Copy;
        _ = AddItemsAsync(items);
    }

    private void OnSearchBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (
            ViewModel.IsContactsView
            && e.KeyModifiers == KeyModifiers.None
            && (e.Key == Key.Down || e.Key == Key.Enter)
        )
        {
            ContactsList.SelectedIndex = 0;
            ContactsList.FocusFirstChildEventually();
            e.Handled = true;
        }
    }

    private void ContactsList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox { SelectedItem: ContactViewModel contact })
        {
            ViewModel.SelectedContact = contact;
        }
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyModifiers == KeyModifiers.Control)
        {
            if (e.Key == Key.O)
            {
                _ = OpenFilesAsync();
                e.Handled = true;
                return;
            }
            int offset = e.Key switch
            {
                Key.PageDown => 1,
                Key.PageUp => -1,
                _ => 0,
            };
            if (offset == 0)
            {
                return;
            }
            SidebarView[] views = Enum.GetValues<SidebarView>();
            int currentIndex = Array.IndexOf(views, ViewModel.SelectedSidebarView);
            ViewModel.SelectedSidebarView = views[(currentIndex + offset + views.Length) % views.Length];
            e.Handled = true;
            return;
        }
        if (e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Shift) && e.Key == Key.O)
        {
            _ = OpenFoldersAsync();
            e.Handled = true;
            return;
        }
        if (e.KeyModifiers == KeyModifiers.None && (e.Key == Key.OemQuestion || e.Key == Key.Divide))
        {
            if (!SearchBox.IsFocused)
            {
                SearchBox.Focus();
                SearchBox.SelectAll();
                e.Handled = true;
                return;
            }
        }
    }

    private async Task AddItemsAsync(IEnumerable<IStorageItem> items)
    {
        await ViewModel.AddFilesAsync(items.Select(item => item.TryGetLocalPath() ?? item.Path.ToString()));
    }

    private void ContactsChip_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.SelectedSidebarView = SidebarView.Contacts;
    }

    private void FilesChip_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.SelectedSidebarView = SidebarView.Files;
    }

    private void RemoveFileButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: VCardFileViewModel file })
        {
            ViewModel.RemoveFile(file);
        }
    }

    private async void OpenContactsButton_Click(object? sender, RoutedEventArgs e)
    {
        await OpenFilesAsync();
    }

    private async Task OpenFilesAsync()
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(
            UIUtil.CreateVCardFilePickerOpenOptions()
        );

        await AddItemsAsync(files);
    }

    private async void OpenFoldersButton_Click(object? sender, RoutedEventArgs e)
    {
        await OpenFoldersAsync();
    }

    private async Task OpenFoldersAsync()
    {
        IReadOnlyList<IStorageFolder> folders = await StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions { Title = Strings.open_folder_title, AllowMultiple = true }
        );

        await AddItemsAsync(folders);
    }

    private async void SettingsButton_Click(object? sender, RoutedEventArgs e)
    {
        await new SettingsDialog().ShowAsync();
    }
}
