using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using NovaAlert.Common;
using DevExpress.Xpf.Core;
using System.Diagnostics;
using NovaAlert.Bll;

namespace NovaAlert.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        const string ResourceFile = "ResourceDictionary.xaml";       

        public App()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {           
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            base.OnStartup(e);
            
            ThemeManager.ApplicationThemeName = Theme.HybridApp.Name;            

            try
            {
                Process proc = Process.GetCurrentProcess();
                int count = Process.GetProcesses().Where(p => p.ProcessName == proc.ProcessName).Count();
                if (count > 1)
                {
                    MessageBox.Show("Chương trình bàn điều khiển đang chạy.");
                    App.Current.Shutdown();
                    return;
                }

                // Message box button text
                DevExpress.Xpf.Editors.EditorLocalizer.Active = new VNDXEditorLocalizer();
                DevExpress.Xpf.Core.DXMessageBoxLocalizer.Active = new VNDXMessageBoxLocalizer();
                InitializeContainer();
                LoadResource();
                var mainWindow = new MainWindow();
                mainWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fail to load resource.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void LoadResource()
        {
            // Clear any previous dictionaries loaded
            Resources.MergedDictionaries.Clear();
            AddMergedResourceFromFile(ResourceFile);            
        }

        private void AddMergedResourceFromFile(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Read in ResourceDictionary File
                var dic = (ResourceDictionary)XamlReader.Load(fs);

                // Add in newly loaded Resource Dictionary
                Resources.MergedDictionaries.Add(dic);
            }
        }

        private static void InitializeContainer()
        {
            ServiceContainer.Instance.AddService<IMessageBoxService>(new NovaAlert.Common.DXMessageBoxService());
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogService.Logger.Error("Dispatcher exception", e.Exception);
            e.Handled = true;
            if (e.Exception is ServiceException)
            {
                return;
            }

            ServiceContainer.Instance.GetService<IMessageBoxService>().ShowError(e.Exception.Message);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogService.Logger.Error("Unhandled exception", e.ExceptionObject as Exception);
        }
    }
}
