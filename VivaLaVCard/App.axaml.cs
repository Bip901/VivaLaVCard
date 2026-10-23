using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

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
                window.ViewModel.AddFiles(desktop.Args);
            }
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
