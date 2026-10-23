using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using FolkerKinzel.VCards;
using FolkerKinzel.VCards.Models.Properties;
using VivaLaVCard.Models;

namespace VivaLaVCard.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    public IReadOnlyList<ContactViewModel> FilteredContacts { get; private set; } = [];

    public IReadOnlyList<VCardFileViewModel> FilteredFiles { get; private set; } = [];

    private readonly List<VCardFileViewModel> files = [];

    public string SearchQuery
    {
        get => searchQuery;
        set
        {
            if (searchQuery == value)
            {
                return;
            }
            searchQuery = value;
            OnPropertyChanged();
            Refilter();
        }
    }
    private string searchQuery = string.Empty;

    public SidebarView SelectedSidebarView
    {
        get => selectedSidebarView;
        set
        {
            if (selectedSidebarView == value)
            {
                return;
            }
            selectedSidebarView = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsContactsView));
            OnPropertyChanged(nameof(IsFilesView));
        }
    }
    private SidebarView selectedSidebarView = SidebarView.Contacts;

    public bool IsContactsView => SelectedSidebarView == SidebarView.Contacts;

    public bool IsFilesView => SelectedSidebarView == SidebarView.Files;

    public ContactViewModel? SelectedContact
    {
        get => selectedContact;
        set
        {
            if (selectedContact != value)
            {
                selectedContact = value;
                OnPropertyChanged();
            }
        }
    }

    private ContactViewModel? selectedContact;

    public MainWindowViewModel()
    {
#if DEBUG
        if (Design.IsDesignMode)
        {
            VCard johnDoe = new() { DisplayNames = new TextProperty("John Doe") };
            VCard timothyJimothy = new() { DisplayNames = new TextProperty("Timothy Jimothy") };
            files.Add(new VCardFileViewModel(new VCardFile("/tmp/designer.vcf", [johnDoe, timothyJimothy])));
            Refilter();
        }
#endif
    }

    public void AddFiles(IEnumerable<string> filePaths)
    {
        foreach (string filePath in filePaths)
        {
            files.Add(new VCardFileViewModel(VCardFile.Load(filePath)));
        }
        Refilter();
    }

    public void RemoveFile(VCardFileViewModel file)
    {
        files.Remove(file);
        Refilter();
        SelectedContact = null;
    }

    private void Refilter()
    {
        IEnumerable<VCardFileViewModel> filteredFilesEnumerable = files;
        IEnumerable<ContactViewModel> filteredContactEnumerable = files.SelectMany(file => file.Contacts);
        string searchQuery = this.searchQuery.Trim();
        if (searchQuery.Length != 0)
        {
            filteredFilesEnumerable = filteredFilesEnumerable.Where(file => file.MatchesSearch(searchQuery));
            filteredContactEnumerable = filteredContactEnumerable.Where(contact => contact.MatchesSearch(searchQuery));
        }
        FilteredFiles = filteredFilesEnumerable.ToArray();
        FilteredContacts = filteredContactEnumerable.ToArray();
        OnPropertyChanged(nameof(FilteredFiles));
        OnPropertyChanged(nameof(FilteredContacts));
    }
}
