using System.Linq;
using Avalonia.Media;
using FolkerKinzel.VCards;

namespace VivaLaVCard.ViewModels;

public sealed class ContactViewModel : ViewModelBase
{
    private readonly VCard _vCard;

    private ContactViewModel(VCard vCard)
    {
        _vCard = vCard;
    }

    public string DisplayName =>
        _vCard.DisplayNames?.FirstOrDefault()?.Value ?? _vCard.Phones?.FirstOrDefault()?.Value ?? string.Empty;

    public IImage? Photo => null;

    public static ContactViewModel FromVCard(VCard vCard)
    {
        return new ContactViewModel(vCard);
    }
}
