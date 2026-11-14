using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
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
            if (TryResolve(out ContactViewModel? referredViewModel))
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

    private bool TryResolve([NotNullWhen(true)] out ContactViewModel? referredViewModel)
    {
        referredViewModel = null;
        return reference != null
            && contactViewModelIndex != null
            && reference.TryResolve(contactViewModelIndex, out referredViewModel);
    }

    public void OnTapped(TopLevel? topLevel)
    {
        if (
            reference != null
            && contactViewModelIndex != null
            && reference.TryResolve(contactViewModelIndex, out ContactViewModel? referredViewModel)
            && topLevel?.DataContext is MainWindowViewModel mainWindowViewModel
        )
        {
            mainWindowViewModel.SelectedContact = referredViewModel;
        }
    }
}
