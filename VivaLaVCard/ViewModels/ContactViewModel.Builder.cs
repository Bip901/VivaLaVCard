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

partial class ContactViewModel
{
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
            AddWorkSection();
            AddAddressesSection();
            AddExtraInformationSection();
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
            AddTextField(Strings.name_prefix, JoinValues(name?.Prefixes));
            AddTextField(Strings.name_first, JoinValues(name?.Given));
            AddTextField(Strings.name_middle, JoinValues(name?.Given2));
            AddTextField(Strings.name_last, JoinValues(name?.Surnames));
            AddTextField(Strings.name_suffix, JoinValues(name?.Suffixes));
            AddTextField(Strings.name_phonetic_first, ConsumeNonStandardValue("X-PHONETIC-FIRST-NAME"));
            AddTextField(Strings.name_phonetic_middle, ConsumeNonStandardValue("X-PHONETIC-MIDDLE-NAME"));
            AddTextField(Strings.name_phonetic_last, ConsumeNonStandardValue("X-PHONETIC-LAST-NAME"));
            if (vCard.NickNames != null)
            {
                foreach (StringCollectionProperty? nickNamesProperty in vCard.NickNames)
                {
                    if (nickNamesProperty is null)
                    {
                        continue;
                    }
                    foreach (string nickName in nickNamesProperty.Value)
                    {
                        AddTextField(Strings.nickname, nickName);
                    }
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

        private void AddWorkSection()
        {
            if (vCard.Titles != null)
            {
                foreach (TextProperty? title in vCard.Titles)
                {
                    AddTextField(Strings.job_title, title?.Value);
                }
            }
            if (vCard.Organizations != null)
            {
                foreach (OrgProperty? organization in vCard.Organizations)
                {
                    if (organization == null)
                    {
                        continue;
                    }
                    AddTextField(Strings.company, organization.Value.Name);
                    if (organization.Value.Units != null)
                    {
                        AddTextField(Strings.department, string.Join(" / ", organization.Value.Units));
                    }
                }
            }
            FlushSection(Strings.section_title_work);
        }

        private void AddAddressesSection()
        {
            if (vCard.Addresses != null)
            {
                foreach (AddressProperty? address in vCard.Addresses)
                {
                    if (address is null)
                    {
                        continue;
                    }
                    string addressType;
                    if (address.Parameters.PropertyClass == PCl.Home)
                    {
                        addressType = Strings.pcl_home;
                    }
                    else if (address.Parameters.PropertyClass == PCl.Work)
                    {
                        addressType = Strings.pcl_work;
                    }
                    else
                    {
                        addressType = GetNonStandardType(address) ?? Strings.address;
                    }
                    fields.Add(
                        new ContactAddressFieldViewModel(
                            addressType,
                            address.Value.Street.FirstOrDefault(),
                            address.Value.Locality.FirstOrDefault(),
                            address.Value.Region.FirstOrDefault(),
                            address.Value.PostalCode.FirstOrDefault(),
                            address.Value.Country.FirstOrDefault()
                        )
                    );
                }
            }
            FlushSection(Strings.section_title_addresses);
        }

        private static string? GetNonStandardType(VCardProperty? prop)
        {
            if (prop == null)
            {
                return null;
            }
            if (prop.Parameters.NonStandard == null)
            {
                return null;
            }
            foreach (var nonStandard in prop.Parameters.NonStandard)
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

        private void AddExtraInformationSection()
        {
            AddTextField(
                Strings.vcard_version,
                vCard.Version switch
                {
                    VCdVersion.V2_1 => "2.1",
                    VCdVersion.V3_0 => "3.0",
                    VCdVersion.V4_0 => "4.0",
                    _ => vCard.Version.ToString(),
                }
            );
            foreach (KeyValuePair<string, List<string>> pair in nonStandards)
            {
                foreach (string value in pair.Value)
                {
                    AddTextField(pair.Key, value);
                }
            }
            FlushSection(Strings.section_title_extra_information);
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
            fields = [];
        }

        private void AddTextField(string label, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }
            fields.Add(new ContactTextFieldViewModel(label, value.Trim()));
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
