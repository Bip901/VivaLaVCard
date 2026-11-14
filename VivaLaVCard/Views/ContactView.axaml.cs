using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using FolkerKinzel.VCards;
using FolkerKinzel.VCards.Enums;
using FolkerKinzel.VCards.Models;
using VivaLaVCard.ViewModels;

namespace VivaLaVCard.Views;

public partial class ContactView : UserControl
{
    public ContactView()
    {
        InitializeComponent();
        AddHandler(TappedEvent, OnTapped, RoutingStrategies.Bubble);

#if DEBUG
        if (Design.IsDesignMode && DataContext is null)
        {
            VCardBuilder vCardBuilder = VCardBuilder.Create();
            vCardBuilder.DisplayNames.Add("Dr. John Doe");
            NameBuilder nameBuilder = NameBuilder.Create();
            nameBuilder.AddPrefix("Dr");
            nameBuilder.AddGiven("John");
            nameBuilder.AddSurname("Doe");
            vCardBuilder.NameViews.Add(nameBuilder.Build());
            vCardBuilder.NickNames.Add("Johnny");
            vCardBuilder.Phones.Add("+11234", paramters => paramters.PhoneType = Tel.Voice);
            vCardBuilder.Phones.Add("+19999", paramters => paramters.PropertyClass = PCl.Work);
            vCardBuilder.EMails.Add("john@example.com");
            vCardBuilder.Titles.Add("Senior Thingy Doer");
            vCardBuilder.Organizations.Add(new Organization("Acme Corp", ["Department of Thingys"]));
            AddressBuilder addressBuilder = AddressBuilder.Create();
            addressBuilder.AddStreet("Some street");
            addressBuilder.AddLocality("Some locality");
            addressBuilder.AddRegion("Some region ");
            addressBuilder.AddPostalCode("Some postal code");
            addressBuilder.AddCountry("Some country");
            vCardBuilder.Addresses.Add(
                addressBuilder.Build(),
                parameters =>
                {
                    parameters.PropertyClass = PCl.Home;
                }
            );
            vCardBuilder.BirthDayViews.Add(DateAndOrTime.Create(new DateOnly(2000, 1, 13)));
            vCardBuilder.NonStandards.Add(
                "X-ANDROID-CUSTOM",
                "vnd.android.cursor.item/relation;Sis Johanson;13;;;;;;;;;;;;;"
            );
            vCardBuilder.Notes.Add("Johnny my man!\n😍");
            vCardBuilder.Urls.Add("https://example.com");
            vCardBuilder.Urls.Add("https://example.org");
            DataContext = ContactViewModel.FromVCard(vCardBuilder.VCard);
        }
#endif
    }

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        for (Visual? visual = e.Source as Visual; visual is not null; visual = visual.GetVisualParent())
        {
            if (visual is Border { DataContext: ContactRelationFieldViewModel relationField })
            {
                relationField.OnTapped(TopLevel.GetTopLevel(this));
                return;
            }
        }
    }
}
