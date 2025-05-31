using System.Windows.Media;

namespace DemoDesktopApp;
public static class Constants
{
    public const double Width = 1000;
    public const double Height = 800;
    public const double MenuWidth = 300;
    public const double ContentWidth = Width - MenuWidth;

    public static Brush MenuBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2FB475"));

    public static Brush Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43677e"));

    public static List<MenuItem> MenuItems =
        [
            new MenuItem(ViewType.DashBoard,"","Dashboard"),
            new MenuItem(ViewType.Read,"","Read")
        ];

}
public record MenuItem(ViewType ViewType, string IconName, string Name);
public enum ViewType
{
    DashBoard,
    Read,
    Update,
    Create,
    Delete
}
