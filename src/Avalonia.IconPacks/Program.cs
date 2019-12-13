using System;
using Avalonia;
using Avalonia.IconPacks.Views;
using Avalonia.Logging.Serilog;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using Avalonia.Shared.PlatformSupport;

namespace Avalonia.IconPacks
{
    class Program
    {

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {

#if PUBLISHWIN
            var builder = AppBuilder.Configure<App>()
                .UseWin32()
                .UseReactiveUI()
                .UseSkia()
                .LogToDebug();
#elif PUBLISHLINUX
            var builder = AppBuilder.Configure<App>()
                .UseX11()
                .UseReactiveUI()
                .UseSkia()
                .LogToDebug();
#elif PUBLISHOSX
            var builder = AppBuilder.Configure<App>()
                .UseAvaloniaNative()
                .UseReactiveUI()
                .UseSkia()
                .LogToDebug();
#else
            var builder = AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .UseSkia()
                .LogToDebug();
#endif
            return builder;
        }
    }
}
