using SimpleBank.Commands;
using SimpleBank.Data;
using SimpleBank.Help;
using SimpleBank.Model;
using SimpleBank.View;
using SimpleBank.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleBank.Command
{
    /// <summary>
    /// Команда создания клиента
    /// </summary>
    public class CreatePersonCommand : ICommand
    {
        private SimpleBankContext _db;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private MainWindow _mainWindow;
        private ObservableCollection<Person> _persons;

        public CreatePersonCommand(SimpleBankContext simpleBankContext,
                                    ObservableCollection<Person> persons,
                                    MainWindowViewModel mainWindowViewModel,
                                    MainWindow mainWindow)
        {
            _db = simpleBankContext;
            _persons = persons;
            _mainWindowViewModel = mainWindowViewModel;
            _mainWindow = mainWindow;
        }

        public CreatePersonCommand()
        {
        }

        ErrorMessage errorMessage = new ErrorMessage();

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            
            if (parameter is StackPanel)
            {
                var stackPanel = (StackPanel)parameter;
                var childrenStackPanel = stackPanel.Children;

                var textBoxLastName = (TextBox)childrenStackPanel[4];
                var textBoxFirstName = (TextBox)childrenStackPanel[6];
                var textBoxFathersName = (TextBox)childrenStackPanel[8];
                var textBoxPhone = (TextBox)childrenStackPanel[10];
                var textBoxPassportNumber = (TextBox)childrenStackPanel[12];

                if (String.IsNullOrWhiteSpace(textBoxLastName.Text) ||
                    String.IsNullOrWhiteSpace(textBoxFirstName.Text) ||
                    String.IsNullOrWhiteSpace(textBoxFathersName.Text) ||
                    String.IsNullOrWhiteSpace(textBoxPhone.Text) ||
                    String.IsNullOrWhiteSpace(textBoxPassportNumber.Text))
                {
                    errorMessage.MessageShow("Заполните все поля");
                    return false;
                }
            }

            return true;
        }

        public void Execute(object parameter)
        {
            Person person = new Person();

            if(parameter is StackPanel)
            {
                var stackPanel = (StackPanel)parameter;
                var childrenStackPanel = stackPanel.Children;

                var textBoxPersonId = (TextBox)childrenStackPanel[2];
                var textBoxLastName = (TextBox)childrenStackPanel[4];
                var textBoxFirstName = (TextBox)childrenStackPanel[6];
                var textBoxFathersName = (TextBox)childrenStackPanel[8];
                var textBoxPhone = (TextBox)childrenStackPanel[10];
                var textBoxPassportNumber = (TextBox)childrenStackPanel[12];
                bool personId = Int32.TryParse(textBoxPersonId.Text, out int _personId);
                if (personId)
                {
                    person.PersonId = _personId;
                }
                person.LastName = textBoxLastName.Text;
                person.FirstName = textBoxFirstName.Text;
                person.FathersName = textBoxFathersName.Text;
                person.Phone = textBoxPhone.Text;
                person.PassportNumber = textBoxPassportNumber.Text;

                CheckParse checkParse = new CheckParse();
                if (!checkParse.CheckParsePhone(textBoxPhone.Text))
                {
                    errorMessage.MessageShow("Телефон должен содержать 11 цифр");
                    return;
                }
                if (!checkParse.CheckParsePassportNumber(textBoxPassportNumber.Text))
                {
                    errorMessage.MessageShow("Номер паспорта должен содержать 6 цифр");
                    return;
                }

                try
                {
                    using (_db = new SimpleBankContext())
                    {
                        if(_db.Persons.Any(p => p.Phone == person.Phone))
                        {
                            errorMessage.MessageShow("Такой номер телефона уже есть в базе");
                            return;
                        }
                        else if(_db.Persons.Any(p => p.PassportNumber == person.PassportNumber))
                        {
                            errorMessage.MessageShow("Такой номер паспорта уже есть в базе");
                            return;
                        }
                        _persons.Add(person);
                        _db.Persons.Add(person);
                        _db.SaveChanges();
                    }

                    App.mainWindow.lbPersonsItems.ItemsSource = _persons;
                    App.mainWindow.lbPersonsItems.Items.Refresh();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
