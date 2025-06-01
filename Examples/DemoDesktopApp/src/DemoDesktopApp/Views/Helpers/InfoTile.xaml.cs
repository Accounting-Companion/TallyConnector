using System.Windows;
using System.Windows.Controls;

namespace DemoDesktopApp.Views.Helpers
{
    /// <summary>
    /// Interaction logic for InfoTile.xaml
    /// </summary>
    public partial class InfoTile : UserControl
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(InfoTile), new PropertyMetadata(string.Empty));



        public string? Value
        {
            get { return (string?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(InfoTile), new PropertyMetadata(null));


        public InfoTile()
        {
            InitializeComponent();
        }
    }
}
