using NovaAlert.Common.Setting;
using NovaAlert.Common.Wpf;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace NovaAlert.Config.Views
{
    /// <summary>
    /// Interaction logic for ResultDataListView2.xaml
    /// </summary>
    public partial class ResultDataListView2 : Window
    {
        int _tillCounter = 0;
        bool _isScrollingDown;        
        Timer _timer;
        NovaAlert.Config.ViewModels.ResultDataListViewModel2 _vm;
        public ResultDataListView2()
        {
            InitializeComponent();
            this.DataContextChanged += ResultDataListView2_DataContextChanged;            
        }        

        void ResultDataListView2_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _vm = this.DataContext as NovaAlert.Config.ViewModels.ResultDataListViewModel2;
            if (_vm != null)
            {                
                _vm.PropertyChanged += vm_PropertyChanged;
            }
        }

        void vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Results")
            {
                if(_vm.AutoScroll) ResetAutoScroll();                
            }
        }

        private void ExpandAllRows()
        {            
            _grid.BeginDataUpdate();
            int dataRowCount = _vm.Results.Count;
            for (int rowHandle = 0; rowHandle < dataRowCount; rowHandle++)
            {
                _grid.SetMasterRowExpanded(rowHandle, true);
            }
            _grid.EndDataUpdate();
        }

        private void ResetAutoScroll()
        {
            try
            {
                ScrollViewer scrollViewer = (ScrollViewer)((ScrollContentPresenter)view.ScrollContentPresenter).TemplatedParent;
                Application.Current.Dispatcher.BeginInvoke(new Action(() => scrollViewer.ScrollToTop()));
            }
            catch { }
            
            _tillCounter = 0;
            System.Threading.Thread.Sleep(_vm.TimerInterval);
            if (_timer == null)
            {
                _timer = new Timer(_vm.TimerInterval);
                _timer.Elapsed += _timer_Elapsed;
            }
            else
            {
                _timer.Enabled = false;
            }
            
            _isScrollingDown = true;
            _timer.Enabled = true;
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetAutoScroll();
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(_vm.AutoScroll) DoAutoScroll();
            _vm.CheckRefeshData();
        }

        void DoAutoScroll()
        {
            double distance = _vm.ScrollDistance;
            ScrollViewer scrollViewer = (ScrollViewer)((ScrollContentPresenter)view.ScrollContentPresenter).TemplatedParent;

            if (scrollViewer.VerticalOffset == 0 && !_isScrollingDown)
            {
                if (_tillCounter < 5)
                {
                    _tillCounter++;
                }
                else
                {
                    _isScrollingDown = true;
                    _tillCounter = 0;
                }
                return;
            }
            else if (scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight && _isScrollingDown)
            {
                if (_tillCounter < 5)
                {
                    _tillCounter++;
                }
                else
                {
                    _isScrollingDown = false;
                    _tillCounter = 0;
                }
            }
            else
            {
                double offset = scrollViewer.VerticalOffset + (_isScrollingDown ? distance : -distance);
                if (offset < 0)
                {
                    offset = 0;
                }
                else if (offset > scrollViewer.ScrollableHeight)
                {
                    offset = scrollViewer.ScrollableHeight;
                }

                try
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => scrollViewer.ScrollToVerticalOffset(offset)));
                }
                catch
                {
                }
            }
        }

        private void btConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var view = new ResultViewerConfig() { DataContext = _vm };
                if (ModalDialog.ShowControl(view, "Tùy chọn") == true)
                {
                    ClientSetting.Instance.Save();
                }
                else
                {
                    ClientSetting.Instance.Reload();
                }
                ResetAutoScroll();
            }catch (Exception exx)
            {
                MessageBox.Show(exx.Message + Environment.NewLine + exx.StackTrace, "Quyen");
            }
        }
    }
}
