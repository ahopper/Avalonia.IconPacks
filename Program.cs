using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.Platform;
using Avalonia.Shared.PlatformSupport;

namespace Avalonia.IconPacks
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<MainWindow>(() => new Avalonia.IconPacks.ViewModels.MainViewModel());
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            var result = AppBuilder.Configure<App>();

            if (result.RuntimePlatform.GetRuntimeInfo().OperatingSystem == OperatingSystemType.OSX)
            {
                result.UseAvaloniaNative().UseSkia();
            }
            else
            {
                result.UsePlatformDetect();
            }
            return result;
        }
    }
}
