using DevExpress.Xpf.Grid;
using System.Windows.Controls;

namespace NovaAlert.Config.Views
{
    /// <summary>
    /// Interaction logic for GroupListView.xaml
    /// </summary>
    public partial class GroupListView : UserControl
    {
        public GroupListView()
        {
            InitializeComponent();
        }

        private void view_FocusedViewChanged(object sender, DevExpress.Xpf.Grid.FocusedViewChangedEventArgs e)
        {
            int rowHandle = ((GridControl)e.NewView.DataControl).GetMasterRowHandle();
            if (GridControl.InvalidRowHandle == rowHandle)
            {
                return;
            }

            grid.CurrentItem = grid.GetRow(rowHandle);
        }
    }
}
