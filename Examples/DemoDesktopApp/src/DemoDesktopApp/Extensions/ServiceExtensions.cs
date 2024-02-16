using Microsoft.Extensions.DependencyInjection;
using TallyConnector.Services;

namespace DemoDesktopApp.Extensions;
public static partial class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<NavigationState>();

        services.AddSingleton<BaseTallyService>();

      

        services.AddSingleton<ViewModelFactory>();
        services.AddViewModels();
        return services;
    }
}
