using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using VivaLaVCard.Resources;
using VivaLaVCard.ViewModels;

namespace VivaLaVCard;

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
    }

    private static void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = HasFiles(e) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        e.Handled = true;

        IStorageFile[] files = e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().ToArray() ?? [];

        if (files.Length == 0)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        e.DragEffects = DragDropEffects.Copy;
        AddFiles(files);
    }

    private static bool HasFiles(DragEventArgs e)
    {
        return e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().Any() == true;
    }

    private void AddFiles(IEnumerable<IStorageFile> files)
    {
        ViewModel.AddFiles(files.Select(file => file.TryGetLocalPath() ?? file.Path.ToString()));
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
        var files = await StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = Strings.open_files_title,
                AllowMultiple = true,
                FileTypeFilter =
                [
                    new FilePickerFileType(Strings.vcard_files_filter) { Patterns = ["*.vcf"] },
                    new FilePickerFileType(Strings.all_files_filter) { Patterns = ["*"] },
                ],
            }
        );

        if (files.Count == 0)
        {
            return;
        }

        AddFiles(files);
    }
}
