using NovaAlert.Common.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NovaAlert.Gui
{
    /// <summary>
    /// Interaction logic for DivHelpView.xaml
    /// </summary>
    public partial class HelpView : UserControl
    {
        public HelpView()
        {
            InitializeComponent();      
            this.DataContextChanged += HelpView_DataContextChanged;
        }

        void HelpView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = this.DataContext as ViewModelBase;
            if(vm != null)
            {
                vm.PropertyChanged += vm_PropertyChanged;
            }
        }

        void vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HelpItem") _scrollViewer.ScrollToBottom();
        }
    }
}
