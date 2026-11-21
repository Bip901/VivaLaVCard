using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaDialogs.Views;
using FolkerKinzel.VCards;
using FolkerKinzel.VCards.Models.Properties;
using VivaLaVCard.Models;
using VivaLaVCard.Resources;
using VivaLaVCard.Util;

namespace VivaLaVCard.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    public IReadOnlyList<ContactViewModel> FilteredContacts { get; private set; } = [];

    public IReadOnlyList<VCardFileViewModel> FilteredFiles { get; private set; } = [];

    private readonly List<VCardFileViewModel> files = [];
    private readonly ContactViewModelIndex contactViewModelIndex = new();

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
            OnPropertyChanged(nameof(ContactCountLabel));
        }
    }
    private SidebarView selectedSidebarView = SidebarView.Contacts;

    public bool IsContactsView => SelectedSidebarView == SidebarView.Contacts;

    public bool IsFilesView => SelectedSidebarView == SidebarView.Files;

    public string ContactCountLabel
    {
        get
        {
            if (IsFilesView)
            {
                return string.Format(Strings.files_count, FilteredFiles.Count);
            }
            return string.IsNullOrWhiteSpace(searchQuery)
                ? string.Format(Strings.contacts_count, FilteredContacts.Count)
                : string.Format(
                    Strings.contacts_search_count,
                    FilteredContacts.Count,
                    files.Sum(file => file.Contacts.Count)
                );
        }
    }

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

    /// <summary>
    /// Loads the VCards from the given files and adds them to the GUI.
    /// If a path denotes a directory, it's searched non-recursively for vcf files.
    /// </summary>
    public async Task AddFilesAsync(IEnumerable<string> paths)
    {
        HashSet<string> existingPaths = [.. files.Select(file => file.FilePath)];
        foreach (string filePath in paths.SelectMany(VCardFilePaths.Expand))
        {
            if (existingPaths.Contains(filePath))
            {
                continue;
            }
            try
            {
                files.Add(new VCardFileViewModel(VCardFile.Load(filePath), contactViewModelIndex));
            }
            catch (Exception ex)
            {
                SingleActionDialog dialog = new()
                {
                    Message = string.Format(Strings.invalid_file_desc, filePath, ex.Message),
                    ButtonText = Strings.ok,
                };
                await dialog.ShowAsync();
            }
        }
        contactViewModelIndex.Regenerate(files.SelectMany(file => file.Contacts));
        Refilter();
        if (searchQuery.Length == 0 && SelectedContact == null && FilteredContacts.Count == 1)
        {
            // Just loaded a file with only 1 contact, select it.
            SelectedContact = FilteredContacts[0];
        }
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
        OnPropertyChanged(nameof(ContactCountLabel));
    }
}
