using SimpleBank.Data;
using SimpleBank.Help;
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
        public static RecordOperationsWindow recordOperationsWindow;
        public static string connectionString = @"Data Source=C:\repos\SimpleBankWithLog\SimpleBankWithLog\Data\SimpleBank.db;New=False;Compress=True;";
        public static event Action<string, string, int?> RecordOperation;
        public static event Action RefreshListOperations;
        public static RecordOperation recordOperation = new RecordOperation();
        public static RefreshData refreshData = new RefreshData();
        public static GetAbbreviatedName abbreviatedName = new GetAbbreviatedName();

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
            recordOperationsWindow = new RecordOperationsWindow();
            
            registrationWindow.Show();

            base.OnStartup(e);
        }
    }
}
