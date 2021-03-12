using log4net;
using NovaAlert.Common;
using System.Windows;

namespace TestModemGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ServiceContainer.Instance.AddService<ILog>(LogService.Logger);
            this.DataContext = new TestModemGuiViewModel();
        }
    }
}
