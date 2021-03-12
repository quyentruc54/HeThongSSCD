using System;
using System.Windows.Controls;

namespace NovaAlert.Config.Views
{
    /// <summary>
    /// Interaction logic for GroupView.xaml
    /// </summary>
    public partial class GroupView : UserControl
    {
        public GroupView()
        {
            InitializeComponent();
            grid.LayoutUpdated += new EventHandler(grid_LayoutUpdated);
        }

        private void grid_LayoutUpdated(object sender, EventArgs e)
        {
            TextBlock textBlock = DevExpress.Xpf.Core.Native.LayoutHelper.FindElementByName(grid, "PART_GridNewRowText") as TextBlock;
            if (textBlock != null)
            {
                textBlock.Text = String.Empty;
                grid.LayoutUpdated -= new EventHandler(grid_LayoutUpdated);
            }

        }
    }
}
