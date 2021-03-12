using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NovaAlert.Common.Wpf
{
    /// <summary>
    /// Interaction logic for ModalDialog.xaml
    /// </summary>
    public partial class ModalDialog : DXWindow
    {
        FrameworkElement _ctrl;

        public FrameworkElement Control
        {
            get 
            { 
                return _ctrl;
            }
            set
            {
                if (value != null)
                {
                    _ctrl = value;

                    Grid.SetRow(_ctrl, 0);
                    Grid.SetColumn(_ctrl, 0);
                    grid.Children.Add(_ctrl);
                    
                    value.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    value.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    KeyboardNavigation.SetTabIndex(value, 0);
                }
            }
        }        

        public bool ShowButton
        {
            get
            {
                return pnButtons.Visibility == System.Windows.Visibility.Visible;
            }

            set
            {
                if (value)
                {
                    pnButtons.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    pnButtons.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

        public ModalDialog()
        {
            InitializeComponent();
            this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            if (this.Control.DataContext is IModalDialog)
            {
                try
                {
                    ((IModalDialog)this.Control.DataContext).OnOK();
                }
                catch (Exception ex)
                {
                    DXMessageBox.Show(this, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            this.DialogResult = true;
            this.Close();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.Control.DataContext is IModalDialog)
            {
                try
                {
                    ((IModalDialog)this.Control.DataContext).OnCancel();
                }
                catch (Exception ex)
                {
                    DXMessageBox.Show(this, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            this.DialogResult = false;
            this.Close();
        }

        public static bool? ShowControl(FrameworkElement ctrl, string title = "", bool showButton = true, bool maximized = false)
        {
            var f = new ModalDialog();
            if (title != "")
            {
                f.Title = title;
            }

            f.ShowButton = showButton;            
            f.Control = ctrl;
            f.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (maximized)
            {   
                f.SizeToContent = SizeToContent.Manual;
                f.WindowState = WindowState.Maximized;             
            }
            else
            {
                f.WindowState = WindowState.Normal;
                f.SizeToContent = SizeToContent.WidthAndHeight;

                f.ResizeMode = ResizeMode.NoResize;
            }

            var activeWindow = Application.Current.GetActiveWindow();
            if (activeWindow != null)
            {
                f.Owner = activeWindow;
            }

            return f.ShowDialog();
        }

        public static string Input(string title, string s = null)
        {
            TextBox tb = new TextBox();
            tb.Height = 25;
            tb.Width = 500;
            tb.Margin = new Thickness(20, 30, 20, 30);

            if (!string.IsNullOrEmpty(s))
                tb.Text = s;

            if (ShowControl(tb, title) == true)
            {
                return tb.Text;
            }

            return null;
        }

        private void DXWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.ShowButton)
            {
                // hide default close button
                StackPanel stackPanel = LayoutHelper.FindElementByName(sender as FrameworkElement, "stackPanel") as StackPanel;
                if (stackPanel != null)
                {
                    stackPanel.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
