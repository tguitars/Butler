using System;
using System.Collections.Generic;
using System.Windows;
using Butler.Views.Window;

namespace Butler
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : ISingleInstanceApp
    {
        private const string Unique = "My_Unique_Application_String";

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // handle command line arguments of second instance
            return true;
        }

        protected override void OnStartup(StartupEventArgs args)
        {
            Dispatcher.UnhandledException += (sender, e) =>
            {
                //new Window1().Show();
                //var errorMessage = string.Format("An exception occurred: {0}",
                //    e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
                //MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
                var window = new ExceptionMessageBox(e.Exception, "AN UNHANDLED EXCEPTION HAS OCCURED.");
                window.ShowDialog();
            };

            BackgroundRunner.Init();
            BackgroundRunner.Run(Repository.Instance.Refresh);
            BackgroundRunner.Run(Repository.Instance.LoadQueryHierarchy);

            base.OnStartup(args);
        }

        [STAThread]
        public static void Main()
        {
            if (!SingleInstance<App>.InitializeAsFirstInstance(Unique)) return;

            var application = new App();
            application.InitializeComponent();
            application.Run();
            // Allow single instance code to perform cleanup operations
            SingleInstance<App>.Cleanup();
        }
    }
}