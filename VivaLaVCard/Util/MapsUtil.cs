using System;

namespace VivaLaVCard.Util;

public static class MapsUtil
{
    private const string OpenStreetMapSearchUrl = "https://www.openstreetmap.org/search?query={0}";
    private const string GoogleMapsSearchUrl = "https://www.google.com/maps/search/{0}";

    public static string GetSearchUrl()
    {
        Settings settings = Settings.Instance;
        if (settings.MapsProvider == MapsProvider.Custom)
        {
            if (string.IsNullOrWhiteSpace(settings.CustomMapsSearchUrl))
            {
                return GetSearchUrl(MapsProvider.OpenStreetMap);
            }
            else
            {
                return settings.CustomMapsSearchUrl;
            }
        }
        return GetSearchUrl(settings.MapsProvider);
    }

    public static string GetSearchUrl(MapsProvider mapsProvider)
    {
        return mapsProvider switch
        {
            MapsProvider.OpenStreetMap => OpenStreetMapSearchUrl,
            MapsProvider.GoogleMaps => GoogleMapsSearchUrl,
            _ => throw new ArgumentException(null, nameof(mapsProvider)),
        };
    }
}

public enum MapsProvider
{
    Custom = 0,
    OpenStreetMap,
    GoogleMaps,
}
