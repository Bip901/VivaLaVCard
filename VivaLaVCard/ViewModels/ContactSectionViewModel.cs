using System.Collections.Generic;

namespace VivaLaVCard.ViewModels;

public sealed class ContactSectionViewModel
{
    public string Title { get; }

    public IReadOnlyList<ContactFieldViewModel> Fields { get; }

    public ContactSectionViewModel(string title, IReadOnlyList<ContactFieldViewModel> fields)
    {
        Title = title;
        Fields = fields;
    }
}
