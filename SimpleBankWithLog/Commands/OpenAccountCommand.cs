using Microsoft.Data.Sqlite;
using SimpleBank.Data;
using SimpleBank.Help;
using SimpleBank.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleBank.Commands
{
    /// <summary>
    /// Команда создания счета
    /// </summary>
    public class OpenAccountCommand : ICommand
    {
        ObservableCollection<Person> _persons;
        Person person = new Person();
        public event Action<string, string, int?> RecordOperation;
        public event Action RefreshListOperations;

        public OpenAccountCommand(ObservableCollection<Person> persons)
        {
            _persons = persons;
            RecordOperation += App.recordOperation.RecordOperationToBD;
            RefreshListOperations += App.refreshData.RefreshDataToUserOptionsWindow;
        }

        ErrorMessage errorMessage = new ErrorMessage();

        public event EventHandler CanExecuteChanged;

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

                var textBlockAccountId = (TextBlock)childrenStackPanel[1];
                if (String.IsNullOrWhiteSpace(textBlockAccountId.Text))
                {
                    errorMessage.MessageShow("Выберите клиента");
                    return;
                }
                var comboBoxAccountType = (ComboBox)childrenStackPanel[4];
                var choose = (ComboBoxItem)comboBoxAccountType.SelectedItem;
                if (choose == null)
                {
                    errorMessage.MessageShow("Выберите тип счета");
                    return;
                }
                bool checkId = Int32.TryParse(textBlockAccountId.Text, out int accountId);
                if (checkId)
                {
                    person = _persons.Single(p => p.PersonId == accountId);
                }

                switch (choose.Content.ToString())
                {
                    case "Зарплатный":
                        try
                        {
                            if (person.TotalSalaryAccount != null)
                            {
                                errorMessage.MessageShow("Зарплатный счет уже открыт");
                                return;
                            }
                            SQLiteConnection connection = new SQLiteConnection(App.connectionString);
                            connection.Open();
                            string stringQuery = "";
                            //bool checkId = Int32.TryParse(textBlockAccountId.Text, out int salaryAccountId);
                            if (checkId)
                            {
                                stringQuery = "INSERT INTO SalaryAccounts ('SalaryAccountId' , 'SalaryTotal' , 'DateSalaryOpen') " +
                                                      "VALUES ('" + accountId + "' , '" +
                                                      0 + "' , '" + DateTime.Now + "')";
                            }
                            else
                            {
                                errorMessage.MessageShow("Некорректный Id");
                                return;
                            }
                            var SqliteCmd = new SQLiteCommand();
                            SqliteCmd = connection.CreateCommand();
                            SqliteCmd.CommandText = stringQuery;
                            SqliteCmd.ExecuteNonQuery();

                            stringQuery = "UPDATE Persons SET TotalSalaryAccount=0 WHERE PersonId="
                                            + accountId + "";
                            SqliteCmd.CommandText = stringQuery;
                            SqliteCmd.ExecuteNonQuery();
                            connection.Close();

                            person.TotalSalaryAccount = 0;

                            string info = "Создание зарплатного счета клиента : " + App.abbreviatedName.GetFIO(person);

                            RecordOperation?.Invoke(App.mainWindow.Title, info, null);
                            App.refreshData.RefreshDataPersons();
                            RefreshListOperations?.Invoke();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            errorMessage.MessageShow("Не удалось подключиться к базе данных");
                        }
                        break;
                    case "Депозитный":
                        try
                        {
                            if (person.TotalDepositAccount != null)
                            {
                                errorMessage.MessageShow("Депозитный счет уже открыт");
                                return;
                            }
                            SQLiteConnection connection = new SQLiteConnection(App.connectionString);
                            connection.Open();
                            string stringQuery = "";
                            if (checkId)
                            {
                                stringQuery = "INSERT INTO DepositAccounts ('DepositAccountId' , 'DepositTotal' , 'DateDepositOpen') " +
                                                      "VALUES ('" + accountId + "' , '" +
                                                      0 + "' , '" + DateTime.Now + "')";
                            }
                            else
                            {
                                errorMessage.MessageShow("Некорректный Id");
                                return;
                            }
                            var SqliteCmd = new SQLiteCommand();
                            SqliteCmd = connection.CreateCommand();
                            SqliteCmd.CommandText = stringQuery;
                            SqliteCmd.ExecuteNonQuery();

                            stringQuery = "UPDATE Persons SET TotalDepositAccount=0 WHERE PersonId="
                                                + accountId + "";
                            SqliteCmd.CommandText = stringQuery;
                            SqliteCmd.ExecuteNonQuery();
                            connection.Close();

                            person.TotalDepositAccount = 0;

                            string info = "Создание депозитного счета клиента : " + App.abbreviatedName.GetFIO(person);

                            RecordOperation?.Invoke(App.mainWindow.Title, info, null);
                            App.refreshData.RefreshDataPersons();
                            RefreshListOperations?.Invoke();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            errorMessage.MessageShow("Не удалось подключиться к базе данных");
                        }
                        break;
                    default:
                        errorMessage.MessageShow("Неопределенный тип счета");
                        break;
                }
            }
        }
    }
}
