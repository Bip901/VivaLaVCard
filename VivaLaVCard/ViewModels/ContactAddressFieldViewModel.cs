using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using VivaLaVCard.Resources;
using VivaLaVCard.Util;

namespace VivaLaVCard.ViewModels;

public sealed class ContactAddressFieldViewModel : ContactFieldViewModel
{
    public string Type { get; }

    public string Street { get; }

    /// <summary>
    /// City
    /// </summary>
    public string Locality { get; }

    public string Region { get; }

    public string PostalCode { get; }

    public string Country { get; }

    public ContactAddressFieldViewModel(
        string type,
        string? street,
        string? locality,
        string? region,
        string? postalCode,
        string? country
    )
    {
        Type = type;
        Street = street ?? string.Empty;
        Locality = locality ?? string.Empty;
        Region = region ?? string.Empty;
        PostalCode = postalCode ?? string.Empty;
        Country = country ?? string.Empty;
    }

#if DEBUG
    // For designer
    public ContactAddressFieldViewModel()
        : this(Strings.pcl_home, "Some street", "Some locality", "Some region", "Some postal code", "Some country") { }
#endif

    public void CopyStreetButtonClickCommand(TopLevel? topLevel)
    {
        _ = CopyToClipboardAsync(topLevel, Street);
    }

    public void CopyLocalityButtonClickCommand(TopLevel? topLevel)
    {
        _ = CopyToClipboardAsync(topLevel, Locality);
    }

    public void CopyRegionButtonClickCommand(TopLevel? topLevel)
    {
        _ = CopyToClipboardAsync(topLevel, Region);
    }

    public void CopyPostalCodeButtonClickCommand(TopLevel? topLevel)
    {
        _ = CopyToClipboardAsync(topLevel, PostalCode);
    }

    public void CopyCountryButtonClickCommand(TopLevel? topLevel)
    {
        _ = CopyToClipboardAsync(topLevel, Country);
    }

    public void MapButtonClickCommand(TopLevel? topLevel)
    {
        ILauncher? launcher = topLevel?.Launcher;
        if (launcher == null)
        {
            return;
        }
        List<string> parts = [Street, Locality, Region, Country];
        for (int i = 0; i < parts.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(parts[i]))
            {
                parts.RemoveAt(i);
                i--;
                continue;
            }
        }
        string query = Uri.EscapeDataString(string.Join(", ", parts));
        Uri uri = new(string.Format(MapsUtil.GetSearchUrl(), query));
        _ = launcher.LaunchUriAsync(uri);
    }
}
