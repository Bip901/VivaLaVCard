using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VivaLaVCard.Util;

/// <summary>
/// Resolves the paths supplied by the user (command line arguments, drag-and-drop) into vCard file paths.
/// </summary>
public static class VCardFilePaths
{
    private static readonly string[] Extensions = [".vcf", ".vcard"];

    public static IEnumerable<string> Expand(string path)
    {
        if (!Directory.Exists(path))
        {
            return [path];
        }
        return Directory
            .EnumerateFiles(path)
            .Where(IsVCardFile)
            .OrderBy(filePath => filePath, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static bool IsVCardFile(string filePath)
    {
        return Extensions.Contains(Path.GetExtension(filePath), StringComparer.OrdinalIgnoreCase);
    }
}
