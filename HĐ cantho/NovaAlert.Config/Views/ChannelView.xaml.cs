using DevExpress.Xpf.Grid;
using System.Windows.Controls;

namespace NovaAlert.Config.Views
{
    /// <summary>
    /// Interaction logic for ChannelView.xaml
    /// </summary>
    public partial class ChannelView : UserControl
    {
        public ChannelView()
        {
            InitializeComponent();
        }

        private void view_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            (sender as TableView).PostEditor();
        }
    }
}
