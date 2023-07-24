using SimpleBank.Data;
using SimpleBank.Model;
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

            mainWindow = new MainWindow();

            _mainWindowViewModel = new MainWindowViewModel();

            personViewModel = new PersonViewModel(_mainWindowViewModel);

            mainWindow.DataContext = _mainWindowViewModel;
            
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}
