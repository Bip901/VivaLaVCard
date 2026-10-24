using System;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using VivaLaVCard.Resources;

namespace VivaLaVCard.ViewModels;

public sealed partial class ContactEmailFieldViewModel : ContactTextFieldViewModel
{
    public ContactEmailFieldViewModel(string label, string value)
        : base(label, value) { }

#if DEBUG
    // For designer
    public ContactEmailFieldViewModel()
        : this(Strings.email, "user@example.com") { }
#endif

    [RelayCommand]
    public void MailButtonClickCommand(TopLevel? topLevel)
    {
        ILauncher? launcher = topLevel?.Launcher;
        if (launcher == null)
        {
            return;
        }
        string[] parts = Value.Split('@');
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = Uri.EscapeDataString(parts[i]);
        }
        string escaped = string.Join('@', parts);
        if (!Uri.TryCreate($"mailto:{escaped}", UriKind.Absolute, out Uri? uri))
        {
            return;
        }
        _ = launcher.LaunchUriAsync(uri);
    }
}
