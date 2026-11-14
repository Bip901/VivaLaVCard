using Avalonia.Media;

namespace VivaLaVCard.ViewModels;

public class ContactRelationFieldViewModel : ContactFieldViewModel
{
    public string RelationKind { get; }

    public string RelativeDisplayName { get; }

    public IImage? Photo => null;

    public ContactRelationFieldViewModel(string relationKind, string relativeDisplayName)
    {
        RelationKind = relationKind;
        RelativeDisplayName = relativeDisplayName;
    }

#if DEBUG
    // For designer
    public ContactRelationFieldViewModel()
        : this("Mother", "Johanna") { }
#endif
}
