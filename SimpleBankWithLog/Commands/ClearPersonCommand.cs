using SimpleBank.Data;
using SimpleBank.Help;
using SimpleBank.Model;
using SimpleBank.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleBank.Commands
{
    /// <summary>
    /// Команда очистки полей ввода
    /// </summary>
    public class ClearPersonCommand : ICommand
    {
        private SimpleBankContext _db;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private MainWindow _mainWindow;
        private ObservableCollection<Person> _persons;
        private int _selectedIndexPerson;

        public event EventHandler CanExecuteChanged;

        public ClearPersonCommand(SimpleBankContext simpleBankContext,
                                    ObservableCollection<Person> persons,
                                    MainWindowViewModel mainWindowViewModel,
                                    MainWindow mainWindow)
        {
            _db = simpleBankContext;
            _persons = persons;
            _mainWindowViewModel = mainWindowViewModel;
            _mainWindow = mainWindow;
        }

        public ClearPersonCommand(int selectedIndexPerson)
        {
            _selectedIndexPerson = selectedIndexPerson;
        }

        public ClearPersonCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is StackPanel)
            {
                var stackPanel = (StackPanel)parameter;
                var childrenStackPanel = stackPanel.Children;

                var textBoxSelectedIndexPerson = (TextBox)childrenStackPanel[1];
                var textBoxPersonId = (TextBox)childrenStackPanel[2];
                var textBoxLastName = (TextBox)childrenStackPanel[4];
                var textBoxFirstName = (TextBox)childrenStackPanel[6];
                var textBoxFathersName = (TextBox)childrenStackPanel[8];
                var textBoxPhone = (TextBox)childrenStackPanel[10];
                var textBoxPassportNumber = (TextBox)childrenStackPanel[12];

                textBoxSelectedIndexPerson.Text = "";
                textBoxPersonId.Text = "";
                textBoxLastName.Text = "";
                textBoxFirstName.Text = "";
                textBoxFathersName.Text = "";
                textBoxPhone.Text = "";
                textBoxPassportNumber.Text = "";
            }
            else return;
        }
    }
}
