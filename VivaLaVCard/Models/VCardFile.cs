using System.Collections.Generic;
using FolkerKinzel.VCards;
using FolkerKinzel.VCards.Models.Properties;

namespace VivaLaVCard.Models;

/// <summary>
/// Represents a vCard file and the vCards that were loaded from it.
/// </summary>
public sealed class VCardFile
{
    public VCardFile(string filePath, IReadOnlyList<VCard> vCards)
    {
        FilePath = filePath;
        VCards = vCards;
    }

    public string FilePath { get; }

    public IReadOnlyList<VCard> VCards { get; }

    public static VCardFile Load(string filePath)
    {
        return new VCardFile(filePath, [new VCard() { DisplayNames = new TextProperty("Example") }]);
    }
}
