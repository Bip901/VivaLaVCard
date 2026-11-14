using System.Collections.Generic;

namespace VivaLaVCard.ViewModels;

public class ContactViewModelIndex
{
    public IReadOnlyDictionary<string, ContactViewModel> ByDisplayName => byDisplayName;
    private readonly Dictionary<string, ContactViewModel> byDisplayName;

    private ContactViewModelIndex(Dictionary<string, ContactViewModel> byDisplayName)
    {
        this.byDisplayName = byDisplayName;
    }

    public static ContactViewModelIndex Generate(IEnumerable<ContactViewModel> viewModels)
    {
        Dictionary<string, ContactViewModel> byDisplayName = [];
        foreach (ContactViewModel viewModel in viewModels)
        {
            if (byDisplayName.ContainsKey(viewModel.DisplayName))
            {
                continue;
            }
            byDisplayName[viewModel.DisplayName] = viewModel;
        }
        return new ContactViewModelIndex(byDisplayName);
    }
}
