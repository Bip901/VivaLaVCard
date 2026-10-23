using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using FolkerKinzel.VCards;
using VivaLaVCard.ViewModels;

namespace VivaLaVCard.Views;

public partial class ContactView : UserControl
{
    public ContactView()
    {
        InitializeComponent();
#if DEBUG
        if (Design.IsDesignMode && DataContext is null)
        {
            VCardBuilder vCardBuilder = VCardBuilder.Create();
            vCardBuilder.DisplayNames.Add("John Doe");
            NameBuilder nameBuilder = NameBuilder.Create();
            nameBuilder.AddPrefix("Dr");
            nameBuilder.AddGiven("John");
            nameBuilder.AddGiven2("Danger");
            nameBuilder.AddSurname("Doe");
            nameBuilder.AddSuffix("The Third");
            vCardBuilder.NameViews.Add(nameBuilder.Build());
            vCardBuilder.NickNames.Add("Johnny");
            DataContext = new MainWindowViewModel { SelectedContact = ContactViewModel.FromVCard(vCardBuilder.VCard) };
        }
#endif
    }

    private async void CopyButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: ContactFieldViewModel field })
        {
            return;
        }
        IClipboard? clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard == null)
        {
            return;
        }
        await clipboard.SetTextAsync(field.Value);
    }
}
