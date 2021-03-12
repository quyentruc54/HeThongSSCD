using NovaAlert.Config.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace NovaAlert.Config.Views
{
    /// <summary>
    /// Interaction logic for ContactView.xaml
    /// </summary>
    public partial class ContactView : UserControl
    {
        public ContactView()
        {
            InitializeComponent();
        }

        private void ComboBoxEdit_ProcessNewValue(DependencyObject sender, DevExpress.Xpf.Editors.ProcessNewValueEventArgs e)
        {
            var vm = DataContext as ContactItemViewModel;
            if (vm != null)
            {
                vm.NameAbbrList.Add(e.DisplayText);
            }
        }

        private void ComboBoxEdit_ProcessNewValue_1(DependencyObject sender, DevExpress.Xpf.Editors.ProcessNewValueEventArgs e)
        {
            var vm = DataContext as ContactItemViewModel;
            if (vm != null)
            {
                vm.UnitNameList.Add(e.DisplayText);
            }
        }
    }
}
