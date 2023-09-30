using SimpleBank.Data;
using SimpleBank.Help;
using SimpleBank.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleBank.Commands
{
    /// <summary>
    /// Команда снятия денег со счета
    /// </summary>
    public class WithdrawMoneyCommand : ICommand
    {
        ObservableCollection<Person> _persons;
        Person person = new Person();
        int newTotal;
        private event Action<string, string, int?> RecordOperation;
        private event Action RefreshListOperations;

        public WithdrawMoneyCommand(ObservableCollection<Person> persons)
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


                var textBoxInputNumber = (TextBox)childrenStackPanel[8];

                bool parseTextBoxInputNumber = Int32.TryParse(textBoxInputNumber.Text, out int inputNumber);
                if (!parseTextBoxInputNumber)
                {
                    errorMessage.MessageShow("Введите число не более суммы на счете");
                    return;
                }

                switch (choose.Content.ToString())
                {
                    case "Зарплатный":
                        try
                        {
                            if (person.TotalSalaryAccount == null)
                            {
                                errorMessage.MessageShow("Откройте счет и внесите сумму");
                                return;
                            }
                            SQLiteConnection connection = new SQLiteConnection(App.connectionString);
                            connection.Open();
                            string stringQuery = "";

                            if (checkId)
                            {
                                stringQuery = "SELECT TotalSalaryAccount FROM Persons WHERE PersonId=" + accountId + "";
                            }
                            else
                            {
                                errorMessage.MessageShow("Некорректный Id");
                                connection.Close();
                                return;
                            }
                            var SqliteCmd = new SQLiteCommand();
                            SqliteCmd.Connection = connection;
                            SqliteCmd.CommandText = stringQuery;
                            var result = SqliteCmd.ExecuteScalar();

                            bool convertTotalSalary = Int32.TryParse(result.ToString(), out int totalSalary);
                            if (convertTotalSalary && parseTextBoxInputNumber)
                            {
                                newTotal = totalSalary - inputNumber;
                                if (newTotal < 0)
                                {
                                    errorMessage.MessageShow("Введенная сумма больше остатка по счету");
                                    connection.Close();
                                    return;
                                }
                                
                            }

                            
                            if (person.TotalSalaryAccount != null && newTotal >= 0)
                            {
                                stringQuery = "UPDATE Persons SET TotalSalaryAccount=" + newTotal + " WHERE PersonId=" + accountId + "";
                                SqliteCmd.CommandText = stringQuery;
                                SqliteCmd.ExecuteNonQuery();
                                connection.Close();
                                person.TotalSalaryAccount = newTotal;

                                string info = "Снятие денег с зарплатного счета клиента : "
                                            + App.abbreviatedName.GetFIO(person);

                                RecordOperation?.Invoke(App.mainWindow.Title, info, inputNumber);
                                App.refreshData.RefreshDataPersons();
                                RefreshListOperations?.Invoke();
                            }
                            else
                            {
                                errorMessage.MessageShow("Введенная сумма превышает остаток по счету");
                                return;
                            }
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
                            if (person.TotalDepositAccount == null)
                            {
                                errorMessage.MessageShow("Откройте счет и внесите сумму");
                                return;
                            }
                            SQLiteConnection connection = new SQLiteConnection(App.connectionString);
                            connection.Open();
                            string stringQuery = "";
                            
                            if (checkId)
                            {
                                stringQuery = "SELECT TotalDepositAccount FROM Persons WHERE PersonId=" + accountId + "";
                            }
                            else
                            {
                                errorMessage.MessageShow("Некорректный Id");
                                connection.Close();
                                return;
                            }
                            var SqliteCmd = new SQLiteCommand();
                            SqliteCmd.Connection = connection;
                            SqliteCmd.CommandText = stringQuery;
                            var result = SqliteCmd.ExecuteScalar();

                            bool convertTotalDeposit = Int32.TryParse(result.ToString(), out int totalDeposit);
                            if (convertTotalDeposit && parseTextBoxInputNumber)
                            {
                                newTotal = totalDeposit - inputNumber;
                                if (newTotal < 0)
                                {
                                    errorMessage.MessageShow("Введенная сумма больше остатка по счету");
                                    connection.Close();
                                    return;
                                }
                                
                            }

                            
                            if (person.TotalDepositAccount != null && newTotal >= 0)
                            {
                                stringQuery = "UPDATE Persons SET TotalDepositAccount=" + newTotal + " WHERE PersonId=" + accountId + "";
                                SqliteCmd.CommandText = stringQuery;
                                SqliteCmd.ExecuteNonQuery();
                                connection.Close();
                                person.TotalDepositAccount = newTotal;

                                string info = "Снятие денег с депозитного счета клиента : "
                                            + App.abbreviatedName.GetFIO(person);

                                RecordOperation?.Invoke(App.mainWindow.Title, info, inputNumber);
                                App.refreshData.RefreshDataPersons();
                                RefreshListOperations?.Invoke();
                            }
                            else
                            {
                                errorMessage.MessageShow("Введенная сумма больше остатка по счету");
                                return;
                            }

                            App.mainWindow.lbPersonsItems.ItemsSource = _persons;
                            App.mainWindow.lbPersonsItems.Items.Refresh();
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
