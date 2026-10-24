using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.TextFormatting.Unicode;

namespace VivaLaVCard.Converters;

public class TextFlowDirectionConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (targetType == typeof(FlowDirection))
        {
            return Convert(value, parameter);
        }
        return App.FlowDir == FlowDirection.RightToLeft ? TextAlignment.Right : TextAlignment.Left;
    }

    private static FlowDirection Convert(object? value, object? parameter)
    {
        if (value is string text && new CodepointEnumerator(text).MoveNext(out Codepoint firstCodepoint))
        {
            switch (firstCodepoint.BiDiClass)
            {
                case BidiClass.RightToLeft:
                case BidiClass.ArabicLetter:
                case BidiClass.ArabicNumber:
                case BidiClass.RightToLeftOverride:
                    //Strong right-to-left preference
                    return FlowDirection.RightToLeft;
                case BidiClass.LeftToRight:
                case BidiClass.LeftToRightOverride:
                    //Strong left-to-right preference
                    return FlowDirection.LeftToRight;
                //default: No strong preference, use the default flow direction
            }
        }
        if (!Enum.TryParse(parameter?.ToString(), out FlowDirection defaultFlowDirection))
        {
            defaultFlowDirection = FlowDirection.LeftToRight;
        }
        return defaultFlowDirection;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
