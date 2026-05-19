using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using System_Resource_Monitor.Services;
using System_Resource_Monitor.ViewModels;

namespace System_Resource_Monitor;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        var window = new MainWindow
        {
            DataContext = Services.GetRequiredService<ShellViewModel>()
        };
        window.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ICpuService, CpuService>();
        services.AddSingleton<IMemoryService, MemoryService>();
        services.AddSingleton<IDiskService, DiskService>();

        services.AddSingleton<CpuViewModel>();
        services.AddSingleton<MemoryViewModel>();
        services.AddSingleton<DiskViewModel>();
        services.AddSingleton<ShellViewModel>();
    }
}
