using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using FolkerKinzel.VCards;
using FolkerKinzel.VCards.Enums;
using VivaLaVCard.ViewModels;

namespace VivaLaVCard.Views;

public partial class ContactView : UserControl
{
    public ContactView()
    {
        InitializeComponent();
#if DEBUG
        if (Design.IsDesignMode && DataContext is null)
        {
            VCardBuilder vCardBuilder = VCardBuilder.Create();
            vCardBuilder.DisplayNames.Add("John Doe");
            NameBuilder nameBuilder = NameBuilder.Create();
            nameBuilder.AddPrefix("Dr");
            nameBuilder.AddGiven("John");
            nameBuilder.AddGiven2("Danger");
            nameBuilder.AddSurname("Doe");
            nameBuilder.AddSuffix("The Third");
            vCardBuilder.NameViews.Add(nameBuilder.Build());
            vCardBuilder.NickNames.Add("Johnny");
            vCardBuilder.Phones.Add("+11234", paramters => paramters.PhoneType = Tel.Voice);
            vCardBuilder.Phones.Add(
                "+14567",
                paramters => paramters.NonStandard = [new KeyValuePair<string, string>("TYPE", "X-TLX")]
            );
            vCardBuilder.Phones.Add("+19999", paramters => paramters.PropertyClass = PCl.Work);
            vCardBuilder.EMails.Add("john@example.com");
            DataContext = ContactViewModel.FromVCard(vCardBuilder.VCard);
        }
#endif
    }

    private async void CopyButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: ContactFieldViewModel field })
        {
            return;
        }
        IClipboard? clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard == null)
        {
            return;
        }
        await clipboard.SetTextAsync(field.Value);
    }

    private async void CallButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: ContactPhoneFieldViewModel field })
        {
            return;
        }
        ILauncher? launcher = TopLevel.GetTopLevel(this)?.Launcher;
        if (launcher == null)
        {
            return;
        }
        string stripped = UeslessTelCharactersRegex().Replace(field.Value, "");
        if (!Uri.TryCreate($"tel:{stripped}", UriKind.Absolute, out Uri? uri))
        {
            return;
        }
        await launcher.LaunchUriAsync(uri);
    }

    [GeneratedRegex("[^0-9+]")]
    private static partial Regex UeslessTelCharactersRegex();

    private async void MailButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: ContactEmailFieldViewModel field })
        {
            return;
        }
        ILauncher? launcher = TopLevel.GetTopLevel(this)?.Launcher;
        if (launcher == null)
        {
            return;
        }
        string[] parts = field.Value.Split('@');
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = Uri.EscapeDataString(parts[i]);
        }
        string escaped = string.Join('@', parts);
        if (!Uri.TryCreate($"mailto:{escaped}", UriKind.Absolute, out Uri? uri))
        {
            return;
        }
        await launcher.LaunchUriAsync(uri);
    }
}
