using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace DemoDesktopApp.Views.Helpers;
/// <summary>
/// Interaction logic for NestedDataGrid.xaml
/// </summary>
public partial class NestedDataGrid : UserControl
{



    public object TallyObject
    {
        get { return (object)GetValue(TallyObjectProperty); }
        set { SetValue(TallyObjectProperty, value); }
    }

    // Using a DependencyProperty as the backing store for TallyObject.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TallyObjectProperty =
        DependencyProperty.Register("TallyObject", typeof(object), typeof(NestedDataGrid), new PropertyMetadata(null));



    public string PropertyName
    {
        get { return (string)GetValue(PropertyNameProperty); }
        set { SetValue(PropertyNameProperty, value); }
    }

    // Using a DependencyProperty as the backing store for PropertyName.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PropertyNameProperty =
        DependencyProperty.Register("PropertyName", typeof(string), typeof(NestedDataGrid), new PropertyMetadata(null));



    public IEnumerable ItemsSource
    {
        get { return (IEnumerable)GetValue(ItemsSourceProperty); }
        set
        {
            SetValue(ItemsSourceProperty, value);
        }
    }

    // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(NestedDataGrid), new PropertyMetadata(null));


    public NestedDataGrid()
    {
        InitializeComponent();
    }
}
