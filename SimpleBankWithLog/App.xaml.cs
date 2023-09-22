using SimpleBank.Data;
using SimpleBank.Model;
using SimpleBank.View;
using SimpleBank.ViewModel;
using System;
using System.Windows;

namespace SimpleBank
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static SimpleBankContext db;
        public static MainWindowViewModel _mainWindowViewModel;
        public static PersonViewModel personViewModel;
        public static MainWindow mainWindow;
        public static RegistrationWindow registrationWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                db = new SimpleBankContext();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            registrationWindow = new RegistrationWindow();
            registrationWindow.Show();

            base.OnStartup(e);
        }
    }
}
