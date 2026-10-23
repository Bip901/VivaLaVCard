namespace VivaLaVCard.Converters;

public static class CustomConverters
{
    public static TextFlowDirectionConverter TextFlowDirectionConverter => _textFlowDirectionConverter ??= new();
    private static TextFlowDirectionConverter? _textFlowDirectionConverter;
}
