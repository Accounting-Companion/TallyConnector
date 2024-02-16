using DemoDesktopApp.Views;
using SaiVineeth.MVVMHelpers.Attributes;
namespace DemoDesktopApp.ViewModels;


[RegisterViews<ViewType, BaseViewModel>]
[RegisterView<DashBoardViewModel, DashboardView>(ViewType.DashBoard)]
public partial class ViewModelFactory
{

}
