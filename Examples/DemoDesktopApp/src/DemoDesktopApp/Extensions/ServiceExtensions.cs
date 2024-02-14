using Microsoft.Extensions.DependencyInjection;

namespace DemoDesktopApp.Extensions;
public static partial class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<NavigationState>();

      

        services.AddSingleton<ViewModelFactory>();
        services.AddViewModels();
        return services;
    }
}
