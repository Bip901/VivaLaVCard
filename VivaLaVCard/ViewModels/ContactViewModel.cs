using System;
using System.Linq;
using Avalonia.Media;
using FolkerKinzel.VCards;

namespace VivaLaVCard.ViewModels;

public sealed class ContactViewModel : ViewModelBase
{
    private readonly VCard vCard;

    private ContactViewModel(VCard vCard)
    {
        this.vCard = vCard;
    }

    public string DisplayName =>
        vCard.DisplayNames?.FirstOrDefault()?.Value ?? vCard.Phones?.FirstOrDefault()?.Value ?? string.Empty;

    public IImage? Photo => null;

    public bool MatchesSearch(string query)
    {
        if (vCard.DisplayNames == null)
        {
            return false;
        }
        return vCard.DisplayNames.Any(displayName =>
            displayName?.Value?.Contains(query, StringComparison.CurrentCultureIgnoreCase) == true
        );
    }

    public static ContactViewModel FromVCard(VCard vCard)
    {
        return new ContactViewModel(vCard);
    }
}
