using SimpleBank.Data;
using SimpleBank.Help;
using SimpleBank.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimpleBank.View
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public static SimpleBankContext db;
        public static MainWindowViewModel _mainWindowViewModel;
        public static PersonViewModel personViewModel;
        public static MainWindow mainWindow;
        private ErrorMessage errorMessage;

        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectUser.SelectedIndex == 0 || SelectUser.SelectedIndex == 1)
            {
                App.mainWindow = new MainWindow();

                _mainWindowViewModel = new MainWindowViewModel();

                personViewModel = new PersonViewModel(_mainWindowViewModel);

                App.mainWindow.DataContext = _mainWindowViewModel;

                this.Hide();

                var selectedUser = (ComboBoxItem)SelectUser.SelectedValue;
                string role = selectedUser.Content.ToString();

                App.mainWindow.Title = role;

                App.mainWindow.Show();
            }
            else
            {
                errorMessage = new ErrorMessage();
                errorMessage.MessageShow("Выберите пользователя");
            }
        }
    }
}
