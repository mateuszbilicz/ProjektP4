using Avalonia;
using Avalonia.ReactiveUI;
using System;

namespace ProjektP4;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        CurrencyValuesContext db = new CurrencyValuesContext();
        db.SyncCurrencyValues();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace().UseReactiveUI();
}