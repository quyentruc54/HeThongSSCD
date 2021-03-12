using System.Timers;
using System.Windows;

namespace NovaAlert.Config.Views
{
    /// <summary>
    /// Interaction logic for ResultDataListView3.xaml
    /// </summary>
    public partial class ResultDataListView3 : Window
    {
        Timer _timer;
        NovaAlert.Config.ViewModels.ResultDataListViewModel3 _vm;
        public ResultDataListView3()
        {
            InitializeComponent();
            this.DataContextChanged += ResultDataListView3_DataContextChanged;
            
        }

        void ResultDataListView3_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _vm = this.DataContext as NovaAlert.Config.ViewModels.ResultDataListViewModel3;
            if(_vm != null)
            {
                _timer = new Timer(10000) { Enabled = true };
                _timer.Elapsed += _timer_Elapsed;
            }
        }
        
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _vm.CheckRefeshData();
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            if(_timer != null && _timer.Enabled)
            {
                _timer.Enabled = false;
                _timer.Dispose();
            }
            
            this.Close();
        }
    }
}
