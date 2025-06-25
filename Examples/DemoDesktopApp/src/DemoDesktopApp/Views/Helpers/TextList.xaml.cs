using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace DemoDesktopApp.Views.Helpers;
/// <summary>
/// Interaction logic for TextList.xaml
/// </summary>
public partial class TextList : UserControl
{
    public IEnumerable ItemsSource
    {
        get { return (IEnumerable)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(TextList), new PropertyMetadata(null));
    public TextList()
    {
        InitializeComponent();
    }
}
