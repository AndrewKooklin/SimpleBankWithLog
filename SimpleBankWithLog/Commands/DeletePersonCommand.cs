using SimpleBank.Data;
using SimpleBank.Help;
using SimpleBank.Model;
using SimpleBank.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace SimpleBank.Commands
{
    /// <summary>
    /// Команда удаления клиента
    /// </summary>
    public class DeletePersonCommand : ICommand
    {
        private SimpleBankContext _db;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private MainWindow _mainWindow;
        private ObservableCollection<Person> _persons;
        private int _selectedIndexPerson;
        public event Action<string, string, int?> RecordOperation;
        public event Action RefreshListOperations;

        public DeletePersonCommand(SimpleBankContext simpleBankContext,
                                    ObservableCollection<Person> persons,
                                    MainWindowViewModel mainWindowViewModel,
                                    MainWindow mainWindow)
        {
            _db = simpleBankContext;
            _persons = persons;
            _mainWindowViewModel = mainWindowViewModel;
            _mainWindow = mainWindow;
            RecordOperation += App.recordOperation.RecordOperationToBD;
            RefreshListOperations += App.refreshData.RefreshDataToUserOptionsWindow;
        }

        public DeletePersonCommand(int selectedIndexPerson)
        {
            _selectedIndexPerson = selectedIndexPerson;
        }

        ErrorMessage errorMessage = new ErrorMessage();

        public event EventHandler CanExecuteChanged;

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

                var textBoxSelectedIndexPerson = (System.Windows.Controls.TextBox)childrenStackPanel[1];
                var textBoxPersonId = (System.Windows.Controls.TextBox)childrenStackPanel[2];
                bool personId = Int32.TryParse(textBoxPersonId.Text, out int _personId);
                if (personId)
                {
                    person.PersonId = _personId;
                }

                try
                {
                    using (_db = new SimpleBankContext())
                    {
                        var personSelected = _db.Persons.Find(person.PersonId);
                        if (personSelected == null)
                        {
                            errorMessage.MessageShow("Выберите клиента из списка");
                            return;
                        }
                        else if(personSelected != null)
                        {
                            DialogResult dialogResult = errorMessage.MessageShowWithResult("При удалении " +
                                "клиента удалятся все его счета." +
                                "\nВы уверены что хотите удалить клиента?");

                            if (dialogResult == DialogResult.No)
                            {
                                return;
                            }
                            else if (dialogResult == DialogResult.Yes)
                            {
                                personSelected.TotalSalaryAccount = null;
                                personSelected.TotalDepositAccount = null;

                                _db.Persons.Remove(personSelected);
                                _db.SaveChanges();

                                bool selectedIndex = Int32.TryParse(textBoxSelectedIndexPerson.Text, out _selectedIndexPerson);

                                if (selectedIndex)
                                {
                                    _persons.RemoveAt(_selectedIndexPerson);

                                    string info = "Удаление клиента : "
                                            + App.abbreviatedName.GetFIO(personSelected);

                                    RecordOperation?.Invoke(App.mainWindow.Title, info, null);
                                    App.refreshData.RefreshDataPersons();
                                    RefreshListOperations?.Invoke();
                                }
                            }
                        }


                        //_db.Persons.Remove(personSelected);
                        //_db.SaveChanges();

                        //bool selectedIndex = Int32.TryParse(textBoxSelectedIndexPerson.Text, out _selectedIndexPerson);

                        //if (selectedIndex)
                        //{

                        //    _persons.RemoveAt(_selectedIndexPerson);

                        //    string info = "Удаление клиента : "
                        //            + App.abbreviatedName.GetFIO(personSelected);

                        //    RecordOperation?.Invoke(App.mainWindow.Title, info, null);
                        //    App.refreshData.RefreshDataPersons();
                        //    RefreshListOperations?.Invoke();
                        //}
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
