using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using VivaLaVCard.Util;
using VivaLaVCard.Views;

namespace VivaLaVCard;

public partial class App : Application
{
    public static FlowDirection FlowDir =>
        CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Settings.Instance.Apply();
            MainWindow window = new();
            if (desktop.Args != null)
            {
                window.DoWhenLoaded(() =>
                {
                    _ = window.ViewModel.AddFilesAsync(desktop.Args);
                });
            }
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
