using DevExpress.Xpf.Grid;
using DevExpress.Mvvm.UI.Interactivity;
using System;
using System.Windows;

namespace NovaAlert.Config.Views
{
    public class DetailCurrentItemBehavior : Behavior<GridControl>
    {
        public static readonly DependencyProperty CurrentItemProperty = DependencyProperty.Register("CurrentItem", typeof(object), typeof(DetailCurrentItemBehavior), new UIPropertyMetadata(null, OnCurrentItemChanged));

        private static void OnCurrentItemChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DetailCurrentItemBehavior detailCurrentItemBehavior = o as DetailCurrentItemBehavior;
            if (detailCurrentItemBehavior != null)
                detailCurrentItemBehavior.OnCurrentItemChanged((object)e.OldValue, (object)e.NewValue);
        }

        protected virtual void OnCurrentItemChanged(object oldValue, object newValue)
        {
            if (isChangeInternal)
            {
                isChangeInternal = false;
                return;
            }
            // Dat comment the following code: no need to auto-expand
            //if (MasterView.MasterRootRowsContainer.FocusedView == MasterView && !MasterGrid.IsMasterRowExpanded(MasterView.FocusedRowHandle))
            //    MasterGrid.ExpandMasterRow(MasterView.FocusedRowHandle);
            //GridControl detailControl = MasterGrid.GetDetail(MasterView.FocusedRowHandle) as GridControl;
            //if (detailControl != null) detailControl.View.MoveFocusedRow(detailControl.DataController.FindRowByRowValue(newValue));
        }

        public object CurrentItem
        {
            get
            {
                return (object)GetValue(CurrentItemProperty);
            }
            set
            {
                SetValue(CurrentItemProperty, value);
            }
        }

        object CurrentItemInternal
        {
            get
            {
                return CurrentItem;
            }
            set
            {
                if (CurrentItem == value)
                    return;
                isChangeInternal = true;
                CurrentItem = value;
            }
        }

        TableView MasterView
        {
            get
            {
                return (DetailView.DataControl.OwnerDetailDescriptor.Parent as GridControl).View as TableView;
            }
        }

        TableView DetailView
        {
            get
            {
                return AssociatedObject.View as TableView;
            }
        }

        GridControl MasterGrid
        {
            get
            {
                return DetailView.DataControl.OwnerDetailDescriptor.Parent as GridControl;
            }
        }

        bool isChangeInternal = false;

        protected override void OnAttached()
        {
            base.OnAttached();
            if (DetailView.DataControl.OwnerDetailDescriptor == null)
            {
                throw new Exception("DetailCurrentItemBehavior should be attached to the detail control");
            }

            AssociatedObject.CurrentItemChanged += DetailGridCurrentItemChanged;
            MasterView.FocusedViewChanged += MasterViewFocusedViewChanged;
            MasterGrid.CurrentItemChanged += MasterGridCurrentItemChanged;
            MasterGrid.MasterRowExpanded += MasterGridMasterRowExpanded;
        }

        private void DetailGridCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            CurrentItemInternal = e.NewItem;
        }

        private void MasterGridCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            if (MasterView.MasterRootRowsContainer.FocusedView == MasterView)
                CurrentItemInternal = null;
        }

        void MasterGridMasterRowExpanded(object sender, RowEventArgs e)
        {
            GridControl detailGrid = MasterGrid.GetDetail(MasterView.FocusedRowHandle) as GridControl;
            if (detailGrid.VisibleRowCount > 0)
            {
                detailGrid.View.MoveFocusedRow(0);
            }
        }

        void MasterViewFocusedViewChanged(object sender, FocusedViewChangedEventArgs e)
        {
            if (e.NewView == MasterView)
            {
                CurrentItemInternal = null;
                return;
            }
            if (e.OldView == MasterView)
            {
                MasterView.FocusedRowHandle = (e.NewView.DataControl as GridControl).GetMasterRowHandle();
            }
            CurrentItemInternal = e.NewView.DataControl.CurrentItem;
        }

        void DetailViewFocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            CurrentItemInternal = e.NewRow;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.CurrentItemChanged -= DetailGridCurrentItemChanged;
            MasterView.FocusedViewChanged -= MasterViewFocusedViewChanged;
            MasterGrid.CurrentItemChanged += MasterGridCurrentItemChanged;
            MasterGrid.MasterRowExpanded -= MasterGridMasterRowExpanded;
            base.OnDetaching();
        }
    }

    public class MasterCurrentItemBehavior : Behavior<GridControl>
    {
        public static readonly DependencyProperty CurrentItemProperty = DependencyProperty.Register("CurrentItem", typeof(object), typeof(MasterCurrentItemBehavior), new UIPropertyMetadata(null, OnCurrentItemChanged));

        private static void OnCurrentItemChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MasterCurrentItemBehavior masterCurrentItemBehavior = o as MasterCurrentItemBehavior;
            if (masterCurrentItemBehavior != null)
            {
                masterCurrentItemBehavior.OnCurrentItemChanged((object)e.OldValue, (object)e.NewValue);
            }
        }

        protected virtual void OnCurrentItemChanged(object oldValue, object newValue)
        {
            if (isChangeInternal)
            {
                isChangeInternal = false;
                return;
            }
            AssociatedObject.View.MoveFocusedRow(AssociatedObject.DataController.FindRowByRowValue(newValue));
        }

        public object CurrentItem
        {
            get
            {
                return (object)GetValue(CurrentItemProperty);
            }
            set
            {
                SetValue(CurrentItemProperty, value);
            }
        }

        object CurrentItemInternal
        {
            get
            {
                return CurrentItem;
            }
            set
            {
                if (CurrentItem == value)
                    return;
                isChangeInternal = true;
                CurrentItem = value;
            }
        }

        bool isChangeInternal = false;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.CurrentItemChanged += AssociatedObject_CurrentItemChanged;
            AssociatedObject.View.FocusedViewChanged += AssociatedObject_FocusedViewChanged;
        }

        private void AssociatedObject_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            CurrentItemInternal = e.NewItem;
        }

        void AssociatedObject_FocusedViewChanged(object sender, FocusedViewChangedEventArgs e)
        {
            AssociatedObject.View.FocusedRowHandle = (e.NewView.DataControl as GridControl).GetMasterRowHandle();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.CurrentItemChanged -= AssociatedObject_CurrentItemChanged;
            base.OnDetaching();
        }
    }
}
