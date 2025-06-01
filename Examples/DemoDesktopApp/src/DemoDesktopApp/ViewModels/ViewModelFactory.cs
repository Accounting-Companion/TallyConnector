using DemoDesktopApp.ViewModels.Prime.v6;
using DemoDesktopApp.Views;
using SaiVineeth.MVVMHelpers.Attributes;
namespace DemoDesktopApp.ViewModels;


[RegisterViews<ViewType, BaseViewModel>]
[RegisterView<DashBoardViewModel, DashboardView>(ViewType.DashBoard)]
[RegisterView<ReadDataViewModel, ReadDataView>(ViewType.Read)]
public partial class ViewModelFactory
{

}
