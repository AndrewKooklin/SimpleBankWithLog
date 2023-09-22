using SimpleBank.Help;
using SimpleBank.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleBank.Commands
{
    public class SelectUserCommand : ICommand
    {
        private RegistrationWindow registrationWindow;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is MainWindow)
            {
                MainWindow mainWindow = (MainWindow)parameter;

                registrationWindow = new RegistrationWindow();

                registrationWindow.Show();

                mainWindow.Close();
            }
            else
            {
                ErrorMessage errorMessage = new ErrorMessage();
                errorMessage.MessageShow("Некорректный параметр");
            }
        }
    }
}
