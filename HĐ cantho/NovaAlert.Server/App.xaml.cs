using System.Linq;
using System.Windows;
using NovaAlert.Common;
using System.Diagnostics;
using DevExpress.Xpf.Core;
using log4net;

namespace NovaAlert.Server
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ThemeManager.ApplicationThemeName = Theme.HybridApp.Name;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            
            Process proc = Process.GetCurrentProcess();
            int count = Process.GetProcesses().Where(p => p.ProcessName == proc.ProcessName).Count();
            if (count > 1)
            {
                MessageBox.Show("Chương trình server đang chạy.");
                App.Current.Shutdown();
                return;
            }

            DevExpress.Xpf.Editors.EditorLocalizer.Active = new VNDXEditorLocalizer();            
            DevExpress.Xpf.Core.DXMessageBoxLocalizer.Active = new VNDXMessageBoxLocalizer();
            ServiceContainer.Instance.AddService<IMessageBoxService>(new NovaAlert.Common.DXMessageBoxService());
            ServiceContainer.Instance.AddService<ILog>(LogService.Logger);
            var mainWindow = new MainWindow();
            mainWindow.ShowDialog();
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {            
            MessageBox.Show(e.Exception.Message);
            LogService.Logger.Fatal("Application Error", e.Exception);
            e.Handled = true;
        }

        internal static void Restart()
        {
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }
    }
}
