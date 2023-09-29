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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleBank.Commands
{
    /// <summary>
    /// Команда изменения данных клиента
    /// </summary>
    public class ChangePersonCommand : ICommand
    {
        private SimpleBankContext _db;
        private MainWindowViewModel _mainWindowViewModel;
        private MainWindow _mainWindow;
        private ObservableCollection<Person> _persons = new ObservableCollection<Person>();
        private int _selectedIndexPerson;

        public event Action<string, string, int?> RecordOperation;
        public event Action RefreshListOperations;
        private RecordOperation recordOperation = new RecordOperation();
        private RefreshData refreshData = new RefreshData();

        public ChangePersonCommand(SimpleBankContext simpleBankContext,
                                    ObservableCollection<Person> persons,
                                    MainWindowViewModel mainWindowViewModel,
                                    MainWindow mainWindow)
        {
            _db = simpleBankContext;
            _persons = persons;
            _mainWindowViewModel = mainWindowViewModel;
            _mainWindow = mainWindow;
            RecordOperation += recordOperation.RecordOperationToBD;
            RefreshListOperations += refreshData.RefreshDataToUserOptionsWindow;
        }

        public ChangePersonCommand(int selectedIndexPerson)
        {
            _selectedIndexPerson = selectedIndexPerson;
        }

        public event EventHandler CanExecuteChanged;

        ErrorMessage errorMessage = new ErrorMessage();

        public bool CanExecute(object parameter)
        {
            
            return true;
        }

        public void Execute(object parameter)
        {
            Person person = new Person();

            if (parameter is StackPanel)
            {
                var stackPanel = (StackPanel)parameter;
                var childrenStackPanel = stackPanel.Children;

                var textBoxSelectedIndexPerson = (TextBox)childrenStackPanel[1];
                if (String.IsNullOrWhiteSpace(textBoxSelectedIndexPerson.Text))
                {
                    errorMessage.MessageShow("Выберите клиента из списка");
                    return;
                }

                var textBoxPersonId = (TextBox)childrenStackPanel[2];
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
                    return;
                }

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

                try
                {
                    using (_db = new SimpleBankContext())
                    {
                        var personSelected = _db.Persons.Find(person.PersonId);
                        if (personSelected == null || textBoxSelectedIndexPerson.Text == "-1")
                        {
                            errorMessage.MessageShow("Выберите клиента из списка");
                            return;
                        }

                        if(person.PersonId != personSelected.PersonId) 
                        {
                            if (_db.Persons.Any(p => p.Phone == person.Phone))
                            {
                                errorMessage.MessageShow("Такой номер телефона уже есть в базе");
                                return;
                            }
                            else if (_db.Persons.Any(p => p.PassportNumber == person.PassportNumber))
                            {
                                errorMessage.MessageShow("Такой номер паспорта уже есть в базе");
                                return;
                            }
                        }

                        var personList = _db.Persons.Where(p => p.PersonId != personSelected.PersonId).ToList();

                        if(personList.Any(p => p.Phone == person.Phone))
                        {
                            errorMessage.MessageShow("Такой номер телефона уже есть в базе");
                            return;
                        }
                        else if (personList.Any(p => p.PassportNumber == person.PassportNumber))
                        {
                            errorMessage.MessageShow("Такой номер паспорта уже есть в базе");
                            return;
                        }

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

                        if(personSelected.LastName != person.LastName ||
                           personSelected.FirstName != person.FirstName ||
                           personSelected.FathersName != person.FathersName ||
                           personSelected.Phone != person.Phone ||
                           personSelected.PassportNumber != person.PassportNumber)
                        {
                            personSelected.LastName = person.LastName;
                            personSelected.FirstName = person.FirstName;
                            personSelected.FathersName = person.FathersName;
                            personSelected.Phone = person.Phone;
                            personSelected.PassportNumber = person.PassportNumber;

                            _db.SaveChanges();

                            refreshData.RefreshDataPersons();

                            string firstLetterFirstName = personSelected.FirstName
                                                                        .ToUpper()
                                                                        .Substring(0, 1);
                            string firstLetterFathersName = personSelected.FathersName
                                                                          .ToUpper()
                                                                          .Substring(0, 1);

                            string info = $"Изменение данных клиента: {personSelected.LastName} " +
                                                                        firstLetterFirstName + "." +
                                                                        firstLetterFathersName + ".";

                            RecordOperation?.Invoke(App.mainWindow.Title, info, null);
                            RefreshListOperations?.Invoke();
                        } 
                    }   
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
