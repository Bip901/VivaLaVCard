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
            string[]? startupPaths =
                (desktop.Args is not null && desktop.Args.Length > 0) ? desktop.Args : Settings.Instance.StartupPaths;
            if (startupPaths is not null && startupPaths.Length > 0)
            {
                window.DoWhenLoaded(() =>
                {
                    _ = window.ViewModel.AddFilesAsync(startupPaths);
                });
            }
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
