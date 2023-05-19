using System.Windows;

namespace NetworkService
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Globalni font
            Style = (Style)FindResource(typeof(Window));
        }
    }
}
