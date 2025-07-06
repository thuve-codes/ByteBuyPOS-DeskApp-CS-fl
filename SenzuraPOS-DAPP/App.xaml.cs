using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace SenzuraPOS_DAPP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Global exception handling
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Set shutdown mode
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Log the exception (e.g., to a file, or a logging service)
            // For now, just show a message box
            MessageBox.Show($"An unhandled exception occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            // Prevent the application from crashing
            e.Handled = true;
        }
    }

}
