namespace VivaLaVCard.ViewModels;

public sealed class ContactFieldViewModel
{
    public ContactFieldViewModel(string? label, string value)
    {
        Label = label;
        Value = value;
    }

    public string? Label { get; }

    public string Value { get; }
}
