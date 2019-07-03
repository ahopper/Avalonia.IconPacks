using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.IconPacks.Views;
using Avalonia.Markup.Xaml;

namespace Avalonia.IconPacks
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow()
                {
                    DataContext = new Avalonia.IconPacks.ViewModels.MainViewModel()
                };
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}
