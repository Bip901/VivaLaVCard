namespace VivaLaVCard.ViewModels;

public class ContactFieldViewModel
{
    public string Label { get; }

    public string Value { get; }

    public ContactFieldViewModel(string label, string value)
    {
        Label = label;
        Value = value;
    }
}
