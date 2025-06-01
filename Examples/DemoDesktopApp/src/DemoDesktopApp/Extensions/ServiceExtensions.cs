using DemoDesktopApp.ViewModels.Prime;
using DemoDesktopApp.ViewModels.Prime.v6;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TallyConnector.Core.Models;
using TallyConnector.Services;

namespace DemoDesktopApp.Extensions;
public static partial class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<NavigationState>();

        services.AddSingleton<TallyConnector.Services.TallyPrime.V6.TallyPrimeService>();

        services.AddSingleton<IBaseTallyService,BaseTallyService>();



        services.AddSingleton<ViewModelFactory>();
        services.AddViewModels();

        services.AddTransient<AbstractDataFetcherViewModel, StockItemsViewModel>();

        services.AddTransient<AbstractDataFetcherViewModel, LedgerViewModel>();
        return services;
    }
}
