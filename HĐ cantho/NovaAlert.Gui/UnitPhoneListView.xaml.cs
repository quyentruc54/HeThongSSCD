using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace NovaAlert.Gui
{
    /// <summary>
    /// Interaction logic for DivUnitView.xaml
    /// </summary>
    public partial class UnitPhoneListView : UserControl
    {
        public UnitPhoneListView()
        {
            InitializeComponent();
            //this.DataContextChanged += UnitPhoneListView_DataContextChanged;
        }

        //void UnitPhoneListView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        //{
        //    itemsControl.Items.SortDescriptions.Clear();
        //    itemsControl.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("ListOrder", System.ComponentModel.ListSortDirection.Descending));
        //}
    }
}
