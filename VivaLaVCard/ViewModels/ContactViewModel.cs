using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using FolkerKinzel.VCards;
using FolkerKinzel.VCards.Models.Properties;

namespace VivaLaVCard.ViewModels;

public sealed partial class ContactViewModel : ViewModelBase
{
    public static ContactViewModel FromVCard(VCard vCard, ContactViewModelIndex? contactViewModelIndex = null)
    {
        return new Builder(vCard).Build(contactViewModelIndex);
    }

    public string DisplayName { get; }

    public IImage? Photo { get; }

    public IReadOnlyList<ContactSectionViewModel> Sections => sections;
    private readonly List<ContactSectionViewModel> sections = [];
    private readonly VCard vCard;

    private ContactViewModel(string displayName, IImage? photo, List<ContactSectionViewModel> sections, VCard vCard)
    {
        DisplayName = displayName;
        Photo = photo;
        this.sections = sections;
        this.vCard = vCard;
    }

    public bool MatchesSearch(string query)
    {
        if (
            AnyTextPropertyContains(vCard.DisplayNames, query)
            || AnyTextPropertyContains(vCard.Phones, query)
            || AnyTextPropertyContains(vCard.EMails, query)
            || AnyTextPropertyContains(vCard.Titles, query)
        )
        {
            return true;
        }
        if (
            vCard.NickNames != null
            && vCard
                .NickNames.SelectMany(nicknames => nicknames?.Value ?? [])
                .Any(nickname => nickname.Contains(query, StringComparison.CurrentCultureIgnoreCase))
        )
        {
            return true;
        }
        if (vCard.Organizations != null)
        {
            foreach (OrgProperty? org in vCard.Organizations)
            {
                if (org is null)
                {
                    continue;
                }
                if (org.Value.Name != null && org.Value.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
                if (
                    org.Value.Units != null
                    && org.Value.Units.Any(unit => unit.Contains(query, StringComparison.CurrentCultureIgnoreCase))
                )
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool AnyTextPropertyContains(IEnumerable<TextProperty?>? properties, string query)
    {
        if (properties == null)
        {
            return false;
        }
        return properties.Any(prop =>
            prop != null && prop.Value.Contains(query, StringComparison.CurrentCultureIgnoreCase)
        );
    }
}
