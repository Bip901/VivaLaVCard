using System;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using VivaLaVCard.Resources;

namespace VivaLaVCard.ViewModels;

public partial class ContactPhoneFieldViewModel : ContactTextFieldViewModel
{
    public ContactPhoneFieldViewModel(string label, string value)
        : base(label, value) { }

#if DEBUG
    // For designer
    public ContactPhoneFieldViewModel()
        : this(Strings.phone, "+11234") { }
#endif

    public virtual void CallButtonClickCommand(TopLevel? topLevel)
    {
        ILauncher? launcher = topLevel?.Launcher;
        if (launcher == null)
        {
            return;
        }
        string stripped = UeslessTelCharactersRegex().Replace(Value, "");
        if (!Uri.TryCreate($"tel:{stripped}", UriKind.Absolute, out Uri? uri))
        {
            return;
        }
        _ = launcher.LaunchUriAsync(uri);
    }

    [GeneratedRegex("[^0-9+]")]
    private static partial Regex UeslessTelCharactersRegex();
}
