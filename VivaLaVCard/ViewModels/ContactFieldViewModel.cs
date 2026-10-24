using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input.Platform;

namespace VivaLaVCard.ViewModels;

public abstract class ContactFieldViewModel
{
    public ContactFieldViewModel() { }

    protected static async Task CopyToClipboardAsync(TopLevel? topLevel, string? value)
    {
        IClipboard? clipboard = topLevel?.Clipboard;
        if (clipboard == null)
        {
            return;
        }
        await clipboard.SetTextAsync(value);
    }
}
