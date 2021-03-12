using NovaAlert.Config.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NovaAlert.Config.Views
{
    /// <summary>
    /// Interaction logic for ContactListView.xaml
    /// </summary>
    public partial class ContactListView : UserControl
    {
        public ContactListView()
        {
            InitializeComponent();
            grid.LayoutUpdated += new EventHandler(grid_LayoutUpdated);
            this.DataContextChanged += ContactListView_DataContextChanged;
        }

        void ContactListView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = this.DataContext as ContactViewModel;
            if (vm != null)
            {
                vm.MakeCallRequestHandler += vm_MakeCallRequestHandler;
            }
        }

        void vm_MakeCallRequestHandler(object sender, EventArgs e)
        {
            var wnd = this.Parent as Window;
            if (wnd != null) wnd.Close();
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
