using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NovaAlert.Gui
{
    /// <summary>
    /// Interaction logic for DivChannelView.xaml
    /// </summary>
    public partial class ChannelListView : UserControl
    {
        public ChannelListView()
        {
            InitializeComponent();
            DataContextChanged += DivChannelView_DataContextChanged;
        }

        private void DivChannelView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
