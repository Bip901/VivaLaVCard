using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;

namespace VivaLaVCard.ViewModels;

public partial class ContactTextFieldViewModel : ContactFieldViewModel
{
    public string Label { get; }

    public string Value { get; }

    public ContactTextFieldViewModel(string label, string value)
    {
        Label = label;
        Value = value;
    }

#if DEBUG
    // For designer
    public ContactTextFieldViewModel()
        : this("Name", "John") { }
#endif

    [RelayCommand]
    public virtual void CopyButtonClickCommand(TopLevel? topLevel)
    {
        _ = CopyToClipboardAsync(topLevel, Value);
    }
}
