using System.Collections.Generic;
using System.IO;
using System.Linq;
using FolkerKinzel.VCards;

namespace VivaLaVCard.Models;

/// <summary>
/// Represents a vCard file and the vCards that were loaded from it.
/// </summary>
public sealed class VCardFile
{
    public string FilePath { get; }

    public VCard[] VCards { get; }

    public VCardFile(string filePath, VCard[] vCards)
    {
        FilePath = filePath;
        VCards = vCards;
    }

    public static VCardFile Load(string filePath)
    {
        using Stream stream = File.OpenRead(filePath);
        return Load(filePath, stream);
    }

    public static VCardFile Load(string filePath, Stream stream)
    {
        using StreamReader streamReader = new(stream);
        VcfReader reader = new(streamReader);
        return new VCardFile(filePath, reader.ReadToEnd().ToArray());
    }
}
