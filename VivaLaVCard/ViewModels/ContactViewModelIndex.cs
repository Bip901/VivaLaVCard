using System.Collections.Generic;

namespace VivaLaVCard.ViewModels;

/// <summary>
/// An index to use when resolving <see cref="ContactViewModelReference"/>s.
/// </summary>
public class ContactViewModelIndex
{
    public IReadOnlyDictionary<string, ContactViewModel> ByDisplayName => byDisplayName;
    private readonly Dictionary<string, ContactViewModel> byDisplayName;

    public ContactViewModelIndex()
    {
        byDisplayName = [];
    }

    public void Regenerate(IEnumerable<ContactViewModel> viewModels)
    {
        byDisplayName.Clear();
        foreach (ContactViewModel viewModel in viewModels)
        {
            if (byDisplayName.ContainsKey(viewModel.DisplayName))
            {
                continue;
            }
            byDisplayName[viewModel.DisplayName] = viewModel;
        }
    }
}
