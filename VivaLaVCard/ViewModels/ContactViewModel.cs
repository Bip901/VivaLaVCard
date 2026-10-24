using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using FolkerKinzel.VCards;
using FolkerKinzel.VCards.Enums;
using FolkerKinzel.VCards.Models;
using FolkerKinzel.VCards.Models.Properties;
using VivaLaVCard.Resources;

namespace VivaLaVCard.ViewModels;

public sealed class ContactViewModel : ViewModelBase
{
    public static ContactViewModel FromVCard(VCard vCard)
    {
        return new Builder(vCard).Build();
    }

    public string DisplayName { get; }

    public IImage? Photo { get; }

    public IReadOnlyList<ContactSectionViewModel> Sections => sections;
    private readonly List<ContactSectionViewModel> sections = [];
    private readonly VCard vCard;

    private ContactViewModel(string displayName, IImage? photo, List<ContactSectionViewModel> sections, VCard vCard)
    {
        DisplayName = displayName;
        Photo = photo;
        this.sections = sections;
        this.vCard = vCard;
    }

    public bool MatchesSearch(string query)
    {
        if (
            AnyTextPropertyContains(vCard.DisplayNames, query)
            || AnyTextPropertyContains(vCard.Phones, query)
            || AnyTextPropertyContains(vCard.EMails, query)
        )
        {
            return true;
        }
        if (
            vCard.NickNames != null
            && vCard
                .NickNames.SelectMany(nicknames => nicknames?.Value ?? [])
                .Any(nickname => nickname.Contains(query, StringComparison.CurrentCultureIgnoreCase))
        )
        {
            return true;
        }
        return false;
    }

    private static bool AnyTextPropertyContains(IEnumerable<TextProperty?>? properties, string query)
    {
        if (properties == null)
        {
            return false;
        }
        return properties.Any(prop =>
            prop != null && prop.Value.Contains(query, StringComparison.CurrentCultureIgnoreCase)
        );
    }

    class Builder
    {
        private readonly VCard vCard;
        private readonly List<ContactSectionViewModel> sections = [];
        private string displayName = string.Empty;
        private List<ContactFieldViewModel> fields = [];
        private readonly Dictionary<string, List<string>> nonStandards = [];
        private IImage? photo;

        public Builder(VCard vCard)
        {
            this.vCard = vCard;
        }

        public ContactViewModel Build()
        {
            displayName =
                vCard.DisplayNames?.FirstOrDefault(property => !string.IsNullOrWhiteSpace(property?.Value))?.Value
                ?? vCard.Phones?.FirstOrDefault(property => !string.IsNullOrWhiteSpace(property?.Value))?.Value
                ?? string.Empty;
            photo = TryLoadPhoto();
            nonStandards.Clear();
            if (vCard.NonStandards != null)
            {
                foreach (var nonStandard in vCard.NonStandards)
                {
                    if (nonStandard == null)
                    {
                        continue;
                    }
                    if (!nonStandards.TryGetValue(nonStandard.Key, out List<string>? values))
                    {
                        values = [];
                        nonStandards[nonStandard.Key] = values;
                    }
                    values.Add(nonStandard.Value);
                }
            }
            AddNameSection();
            AddContactSection();
            AddCustomFieldsSection();
            return new ContactViewModel(displayName, photo, sections, vCard);
        }

        private Bitmap? TryLoadPhoto()
        {
            RawData? rawData = vCard.Photos?.FirstOrDefault(property => property is { IsEmpty: false })?.Value;
            if (rawData is null || rawData.IsEmpty || rawData.Bytes == null)
            {
                return null;
            }
            try
            {
                using MemoryStream stream = new(rawData.Bytes);
                return new Bitmap(stream);
            }
            catch
            {
                return null;
            }
        }

        private void AddNameSection()
        {
            Name? name = vCard.NameViews?.FirstOrDefault(property => property is { IsEmpty: false })?.Value;
            AddField(Strings.name_prefix, JoinValues(name?.Prefixes));
            AddField(Strings.name_first, JoinValues(name?.Given));
            AddField(Strings.name_middle, JoinValues(name?.Given2));
            AddField(Strings.name_last, JoinValues(name?.Surnames));
            AddField(Strings.name_suffix, JoinValues(name?.Suffixes));
            AddField(Strings.name_phonetic_first, ConsumeNonStandardValue("X-PHONETIC-FIRST-NAME"));
            AddField(Strings.name_phonetic_middle, ConsumeNonStandardValue("X-PHONETIC-MIDDLE-NAME"));
            AddField(Strings.name_phonetic_last, ConsumeNonStandardValue("X-PHONETIC-LAST-NAME"));
            foreach (StringCollectionProperty? nickNamesProperty in vCard.NickNames ?? [])
            {
                if (nickNamesProperty?.Value is null)
                {
                    continue;
                }
                foreach (string nickName in nickNamesProperty.Value)
                {
                    AddField(Strings.nickname, nickName);
                }
            }
            FlushSection(Strings.section_title_name);
        }

        private void AddContactSection()
        {
            if (vCard.Phones != null)
            {
                foreach (TextProperty? phone in vCard.Phones)
                {
                    if (phone == null || string.IsNullOrWhiteSpace(phone.Value))
                    {
                        continue;
                    }
                    string? phoneType;
                    if (phone.Parameters.PropertyClass == PCl.Home)
                    {
                        phoneType = Strings.pcl_home;
                    }
                    else if (phone.Parameters.PropertyClass == PCl.Work)
                    {
                        phoneType = Strings.pcl_work;
                    }
                    else if (phone.Parameters.PhoneType == Tel.Cell)
                    {
                        phoneType = Strings.phone_cell;
                    }
                    else
                    {
                        phoneType = GetNonStandardType(phone);
                    }
                    string label = Strings.phone;
                    if (phoneType != null)
                    {
                        label = $"{label} ({phoneType})";
                    }
                    fields.Add(new ContactPhoneFieldViewModel(label, phone.Value));
                }
            }
            if (vCard.EMails != null)
            {
                foreach (TextProperty? email in vCard.EMails)
                {
                    if (email == null || string.IsNullOrWhiteSpace(email.Value))
                    {
                        continue;
                    }
                    string? emailType;
                    if (email.Parameters.PropertyClass == PCl.Home)
                    {
                        emailType = Strings.pcl_home;
                    }
                    else if (email.Parameters.PropertyClass == PCl.Work)
                    {
                        emailType = Strings.pcl_work;
                    }
                    else
                    {
                        emailType = email.Parameters.EMailType;
                        if (emailType == EMail.SMTP)
                        {
                            emailType = null; // "INTERNET" is not interesting information
                        }
                        else if (emailType == "HOME")
                        {
                            emailType = Strings.pcl_home;
                        }
                        else if (emailType == "WORK")
                        {
                            emailType = Strings.pcl_work;
                        }
                    }
                    string label = Strings.email;
                    if (emailType != null)
                    {
                        label = $"{label} ({emailType})";
                    }
                    fields.Add(new ContactEmailFieldViewModel(label, email.Value));
                }
            }
            FlushSection(Strings.section_title_contact);
        }

        private static string? GetNonStandardType(TextProperty? textProperty)
        {
            if (textProperty == null)
            {
                return null;
            }
            if (textProperty.Parameters.NonStandard == null)
            {
                return null;
            }
            foreach (var nonStandard in textProperty.Parameters.NonStandard)
            {
                if (nonStandard.Key != "TYPE")
                {
                    continue;
                }
                string result = nonStandard.Value;
                if (result.StartsWith("X-"))
                {
                    result = result["X-".Length..];
                }
                return result;
            }
            return null;
        }

        private void AddCustomFieldsSection()
        {
            foreach (KeyValuePair<string, List<string>> pair in nonStandards)
            {
                foreach (string value in pair.Value)
                {
                    AddField(pair.Key, value);
                }
            }
            FlushSection(Strings.section_title_custom_fields);
        }

        private string? ConsumeNonStandardValue(string key)
        {
            if (nonStandards.Remove(key, out List<string>? values) && values.Count > 0)
            {
                return values[0];
            }
            return null;
        }

        private void FlushSection(string title)
        {
            if (fields.Count == 0)
            {
                return;
            }
            sections.Add(new ContactSectionViewModel(title, fields));
            fields = new List<ContactFieldViewModel>();
        }

        private void AddField(string label, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }
            fields.Add(new ContactFieldViewModel(label, value.Trim()));
        }

        private static string? JoinValues(IReadOnlyList<string>? values)
        {
            if (values is null)
            {
                return null;
            }
            string joined = string.Join(
                " ",
                values.Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => value.Trim())
            );
            return string.IsNullOrWhiteSpace(joined) ? null : joined;
        }
    }
}
