using Avalonia.Media;

namespace VivaLaVCard.ViewModels;

public class ContactRelationFieldViewModel : ContactFieldViewModel
{
    public string RelationKind { get; }

    public string RelativeDisplayName { get; }

    public IImage? Photo
    {
        get
        {
            if (
                reference != null
                && contactViewModelIndex != null
                && reference.TryResolve(contactViewModelIndex, out ContactViewModel? referredViewModel)
            )
            {
                return referredViewModel.Photo;
            }
            return null;
        }
    }

    private readonly ContactViewModelReference? reference;
    private readonly ContactViewModelIndex? contactViewModelIndex;

    public ContactRelationFieldViewModel(
        string relationKind,
        string relativeDisplayName,
        ContactViewModelReference? reference,
        ContactViewModelIndex? contactViewModelIndex
    )
    {
        RelationKind = relationKind;
        RelativeDisplayName = relativeDisplayName;
        this.reference = reference;
        this.contactViewModelIndex = contactViewModelIndex;
    }

#if DEBUG
    // For designer
    public ContactRelationFieldViewModel()
        : this("Mother", "Johanna", null, null) { }
#endif
}
