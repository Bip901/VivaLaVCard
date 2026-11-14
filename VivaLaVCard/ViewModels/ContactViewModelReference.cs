using System.Diagnostics.CodeAnalysis;

namespace VivaLaVCard.ViewModels;

public abstract class ContactViewModelReference
{
    public abstract bool TryResolve(ContactViewModelIndex index, [NotNullWhen(true)] out ContactViewModel? viewModel);
}

public class ContactViewModelReference_DisplayName : ContactViewModelReference
{
    public string DisplayName { get; }

    public ContactViewModelReference_DisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    public override bool TryResolve(ContactViewModelIndex index, [NotNullWhen(true)] out ContactViewModel? viewModel)
    {
        return index.ByDisplayName.TryGetValue(DisplayName, out viewModel);
    }
}
