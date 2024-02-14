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
        base.OnStartup(e);
        host.Services.GetRequiredService<MainWindow>().Show();
    }
}
