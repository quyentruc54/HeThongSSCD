using System;
using System.Windows;
using NovaAlert.Service;
using System.Windows.Interop;
using NovaAlert.Common;
using System.Windows.Media.Imaging;

namespace NovaAlert.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.NotifyIcon _notifyIcon;
        MainWindowViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();
            _notifyIcon = new System.Windows.Forms.NotifyIcon() 
            {
                Visible = false,
                Text = this.Title
            };

            
            var icon = this.Icon as BitmapSource;
            if (icon != null)
            {                
                var bmp = BitmapSourceToBitmap(icon);
                _notifyIcon.Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon());
            }

            _notifyIcon.MouseDoubleClick += _notifyIcon_MouseDoubleClick;

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
            StateChanged += MainWindow_StateChanged;         
        }

        void _notifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Minimized)
                this.WindowState = System.Windows.WindowState.Normal;
        }

        void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                _notifyIcon.Visible = true;
            }
            else
            {
                this.ShowInTaskbar = true;
                _notifyIcon.Visible = false;
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {   
            try
            {
                CommonResource.Instance.InitSoundChannels(new WindowInteropHelper(this).Handle);
                CommonResource.Instance.InitDialupModem();
                string recordDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Record");
                RecorderManager.InitRecorder(recordDir);
            }
            catch(Exception ex)
            {
                LogService.Logger.Error(ex);
            }

            _vm = new MainWindowViewModel();
            this.DataContext = _vm;

            this.WindowState = System.Windows.WindowState.Minimized;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            _vm.StopCommand.Execute(null);
        }

        public static System.Drawing.Bitmap BitmapSourceToBitmap(System.Windows.Media.Imaging.BitmapSource srs)
        {
            System.Drawing.Bitmap btm = null;
            int width = srs.PixelWidth;
            int height = srs.PixelHeight;
            int stride = width * ((srs.Format.BitsPerPixel + 7) / 8);
            byte[] bits = new byte[height * stride];

            srs.CopyPixels(bits, stride, 0);

            unsafe
            {

                fixed (byte* pB = bits)
                {
                    IntPtr ptr = new IntPtr(pB);

                    btm = new System.Drawing.Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, ptr);

                }
            }
            return btm;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var msg = "Hệ thống sẽ dừng hoạt động. Bạn có chắc chắn hay không ?";
            e.Cancel = ServiceContainer.Instance.GetService<IMessageBoxService>().AskYesNo(msg) == MessageBoxResult.No;            
        }

        private void btnShowLog_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new Views.wndLogViewer();
            wnd.ShowDialog();
        }
    }
}
