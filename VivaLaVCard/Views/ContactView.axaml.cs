using Avalonia.Controls;
using FolkerKinzel.VCards;
using FolkerKinzel.VCards.Models.Properties;
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
            VCard johnDoe = new() { DisplayNames = new TextProperty("John Doe") };
            DataContext = new MainWindowViewModel { SelectedContact = ContactViewModel.FromVCard(johnDoe) };
        }
#endif
    }
}
