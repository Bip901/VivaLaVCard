using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using FolkerKinzel.VCards;
using FolkerKinzel.VCards.Models.Properties;
using VivaLaVCard.Models;

namespace VivaLaVCard.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<VCardFileViewModel> Files { get; }

    private ContactViewModel? _selectedContact;
    private SidebarView _selectedSidebarView = SidebarView.Contacts;

    public MainWindowViewModel()
    {
        Files = [];
        Files.CollectionChanged += OnFilesChanged;
    }

    public static MainWindowViewModel DesignInstance { get; } = CreateDesignInstance();

    public IReadOnlyList<ContactViewModel> Contacts => Files.SelectMany(file => file.Contacts).ToArray();

    public SidebarView SelectedSidebarView
    {
        get => _selectedSidebarView;
        set
        {
            if (_selectedSidebarView == value)
            {
                return;
            }

            _selectedSidebarView = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsContactsView));
            OnPropertyChanged(nameof(IsFilesView));
        }
    }

    public bool IsContactsView => SelectedSidebarView == SidebarView.Contacts;

    public bool IsFilesView => SelectedSidebarView == SidebarView.Files;

    public ContactViewModel? SelectedContact
    {
        get => _selectedContact;
        set
        {
            if (_selectedContact == value)
            {
                return;
            }

            _selectedContact = value;
            OnPropertyChanged();
        }
    }

    public void AddFiles(IEnumerable<string> filePaths)
    {
        foreach (string filePath in filePaths)
        {
            Files.Add(new VCardFileViewModel(VCardFile.Load(filePath)));
        }
    }

    public void RemoveFile(VCardFileViewModel file)
    {
        Files.Remove(file);
    }

    private void OnFilesChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        OnPropertyChanged(nameof(Contacts));
        if (args.Action != NotifyCollectionChangedAction.Add)
        {
            SelectedContact = null;
        }
    }

    // Placeholder data for the XAML designer only.
    private static MainWindowViewModel CreateDesignInstance()
    {
        VCard johnDoe = new() { DisplayNames = new TextProperty("John Doe") };
        VCard timothyJimothy = new() { DisplayNames = new TextProperty("Timothy Jimothy") };
        MainWindowViewModel result = new();
        result.Files.Add(new VCardFileViewModel(new VCardFile("/tmp/designer.vcf", [johnDoe, timothyJimothy])));
        return result;
    }
}
