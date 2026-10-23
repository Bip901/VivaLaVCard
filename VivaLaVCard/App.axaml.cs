using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using VivaLaVCard.Util;
using VivaLaVCard.Views;

namespace VivaLaVCard;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
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
