using System.Windows.Media;

namespace DemoDesktopApp;
public static class Constants
{
    public const double Width = 1000;
    public const double Height = 800;
    public const double MenuWidth = 300;

    public static Brush MenuBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2FB475"));

    public static Brush Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43677e"));

}
public enum ViewType
{
    DashBoard,
    Prime3,
    EditLog,
}
