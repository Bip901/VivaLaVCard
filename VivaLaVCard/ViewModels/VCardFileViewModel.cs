using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VivaLaVCard.Models;

namespace VivaLaVCard.ViewModels;

/// <summary>
/// Presents a <see cref="VCardFile"/> to the GUI without exposing the vCard library.
/// </summary>
public sealed class VCardFileViewModel : ViewModelBase
{
    private readonly VCardFile _file;

    public VCardFileViewModel(VCardFile file, ContactViewModelIndex? contactViewModelIndex = null)
    {
        _file = file;
        Contacts = file.VCards.Select(vCard => ContactViewModel.FromVCard(vCard, contactViewModelIndex)).ToArray();
    }

    public string FilePath => _file.FilePath;

    public string FileName => Path.GetFileName(FilePath);

    public IReadOnlyList<ContactViewModel> Contacts { get; }

    public bool MatchesSearch(string query)
    {
        return FileName.Contains(query, StringComparison.CurrentCultureIgnoreCase);
    }
}
