using DemoDesktopApp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
//using Serilog;
namespace DemoDesktopApp;
public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        AddMVVMTemplatesResourceDict();
        var hostApplicationBuilder = Host.CreateApplicationBuilder();
        hostApplicationBuilder.Services.AddServices();

        //hostApplicationBuilder.lo();


        IHost host = hostApplicationBuilder.Build();
        host.Start();

        var settings = DemoDesktopApp.Models.AppSettings.Load();
        var tallyService = host.Services.GetRequiredService<TallyConnector.Services.TallyPrime.V6.TallyPrimeService>();
        tallyService.SetupTallyService(settings.TallyBaseUrl, settings.TallyPort);

        base.OnStartup(e);
        host.Services.GetRequiredService<MainWindow>().Show();
    }
}
