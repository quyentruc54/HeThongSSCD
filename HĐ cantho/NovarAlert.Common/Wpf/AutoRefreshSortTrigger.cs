using DevExpress.Xpf.Grid;
using System.Linq;
using System.Windows;

namespace NovaAlert.Common.Wpf
{
    public static class AutoRefreshSortTrigger
    {
        public static bool GetAutoRefreshSort(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoRefreshSortProperty);
        }

        public static void SetAutoRefreshSort(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoRefreshSortProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoRefeshSort.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoRefreshSortProperty =
            DependencyProperty.RegisterAttached("AutoRefreshSort", typeof(bool), typeof(AutoRefreshSortTrigger), new PropertyMetadata(false, AutoRefreshChangedCallback));

        static void AutoRefreshChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as GridControl;
            if(grid != null && grid.Columns.Any(c => c.SortOrder != DevExpress.Data.ColumnSortOrder.None))
            {
                grid.BeginDataUpdate();
                grid.EndDataUpdate();
            }
        }
    }
}
