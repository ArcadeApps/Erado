using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Erado.UI.ViewModels;
using Erado.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Erado.UI;

public partial class App : Application
{
    public IHost? GlobalHost { get; private set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        var hostBuilder = CreateHostBuilder();
        var host = hostBuilder.Build();
        GlobalHost = host;
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = GlobalHost.Services.GetRequiredService<MainWindowViewModel>()
            };
            desktop.Exit += (sender, args) =>
            {
                GlobalHost.StopAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
                GlobalHost.Dispose();
                GlobalHost = null;
            };
        }
        
        DataTemplates.Add(GlobalHost.Services.GetRequiredService<ViewLocator>());
        
        base.OnFrameworkInitializationCompleted();
        
        await host.RunAsync();
    }
    private static HostApplicationBuilder CreateHostBuilder()
    {
        var builder = Host.CreateApplicationBuilder(Environment.GetCommandLineArgs());
        
        builder.Services.AddTransient<ViewLocator>();
        builder.Services.AddTransient<MainWindowViewModel>();

        return builder;
    }
}