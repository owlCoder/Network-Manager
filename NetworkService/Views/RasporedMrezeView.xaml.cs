using System.Windows.Controls;

namespace NetworkService.Views
{
    /// <summary>
    /// Interaction logic for RasporedMrezeView.xaml
    /// </summary>
    public partial class RasporedMrezeView : UserControl
    {
        public static UserControl UserControl { get; set; }
        public RasporedMrezeView()
        {
            InitializeComponent();

            UserControl = this;
        }
    }
}
