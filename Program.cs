using System;
using Avalonia;
using Avalonia.Logging.Serilog;

namespace Avalonia.IconPacks
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<MainWindow>(() => new Avalonia.IconPacks.ViewModels.MainViewModel());
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}
