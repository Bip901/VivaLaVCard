using System;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using VivaLaVCard.Resources;

namespace VivaLaVCard.ViewModels;

public sealed class ContactUrlFieldViewModel : ContactTextFieldViewModel
{
    public ContactUrlFieldViewModel(string label, string value)
        : base(label, value) { }

#if DEBUG
    // For designer
    public ContactUrlFieldViewModel()
        : this(Strings.website, "https://example.com") { }
#endif

    public void OpenWebsiteButtonClickCommand(TopLevel? topLevel)
    {
        ILauncher? launcher = topLevel?.Launcher;
        if (launcher == null)
        {
            return;
        }
        if (!Uri.TryCreate(Value, UriKind.RelativeOrAbsolute, out Uri? uri))
        {
            return;
        }
        _ = launcher.LaunchUriAsync(uri);
    }
}
