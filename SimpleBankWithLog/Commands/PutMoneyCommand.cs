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
    /// Команда внесения денег на счет
    /// </summary>
    public class PutMoneyCommand : ICommand
    {
        ObservableCollection<Person> _persons;
        private SalaryAccount salaryAccount;
        private DepositAccount depositAccount;
        Person person = new Person();
        private Account account = new Account();
        public event Action<string, string, int?> RecordOperation;
        public event Action RefreshListOperations;

        public PutMoneyCommand(ObservableCollection<Person> persons)
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
                    errorMessage.MessageShow("Введите положительное число не более 2000000000");
                    return;
                }

                switch (choose.Content.ToString())
                {
                    case "Зарплатный":
                        try
                        {
                            if(person.TotalSalaryAccount == null)
                            {
                                errorMessage.MessageShow("Откройте счет");
                                return;
                            }
                            SQLiteConnection connection = new SQLiteConnection(App.connectionString);
                            connection.Open();
                            string stringQuery = "";
                            
                            //bool checkId = Int32.TryParse(textBlockAccountId.Text, out int salaryAccountId);
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
                            //

                            bool convertTotalSalary = Int32.TryParse(result.ToString(), out int totalSalary);
                            salaryAccount = new SalaryAccount
                            {
                                Total = totalSalary
                            };
                            if (convertTotalSalary && parseTextBoxInputNumber)
                            {
                                account = salaryAccount.PutMoney(salaryAccount, inputNumber);
                                if(account.Total > 2100000000 || account.Total < 0)
                                {
                                    errorMessage.MessageShow("Максимальная сумма на счете 2100000000");
                                    connection.Close();
                                    return;
                                }
                                stringQuery = "UPDATE Persons SET TotalSalaryAccount="+ account.Total + " WHERE PersonId=" + accountId + "";
                                SqliteCmd.CommandText = stringQuery;
                                SqliteCmd.ExecuteNonQuery();
                                connection.Close();

                                string info = "Внесение денег на зарплатный счет клиента : "
                                            + App.abbreviatedName.GetFIO(person);

                                RecordOperation?.Invoke(App.mainWindow.Title, info, inputNumber);
                                App.refreshData.RefreshDataPersons();
                                RefreshListOperations?.Invoke();
                            }

                            if (person.TotalSalaryAccount != null && account.Total < 2100000000 && account.Total > 0)
                            {
                                person.TotalSalaryAccount = account.Total;
                            }
                            else
                            {
                                errorMessage.MessageShow("Максимальная сумма на счете 2100000000");
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
                                errorMessage.MessageShow("Откройте счет");
                                return;
                            }
                            SQLiteConnection connection = new SQLiteConnection(App.connectionString);
                            connection.Open();
                            string stringQuery = "";
                            //bool checkId = Int32.TryParse(textBlockAccountId.Text, out int depositAccountId);
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
                            //

                            bool convertTotalDeposit = Int32.TryParse(result.ToString(), out int totalDeposit);
                            depositAccount = new DepositAccount
                            {
                                Total = totalDeposit
                            };
                            if (convertTotalDeposit && parseTextBoxInputNumber)
                            {
                                account = depositAccount.PutMoney(depositAccount, inputNumber);
                                if (depositAccount.Total > 2100000000 || depositAccount.Total < 0)
                                {
                                    errorMessage.MessageShow("Максимальная сумма на счете 2100000000");
                                    connection.Close();
                                    return;
                                }
                                stringQuery = "UPDATE Persons SET TotalDepositAccount=" + account.Total + " WHERE PersonId=" + accountId + "";
                                SqliteCmd.CommandText = stringQuery;
                                SqliteCmd.ExecuteNonQuery();
                                connection.Close();

                                string info = "Внесение денег на депозитный счет клиента : "
                                            + App.abbreviatedName.GetFIO(person);

                                RecordOperation?.Invoke(App.mainWindow.Title, info, inputNumber);
                                App.refreshData.RefreshDataPersons();
                                RefreshListOperations?.Invoke();
                            }

                            if (person.TotalDepositAccount != null && 
                                account.Total < 2100000000 && 
                                account.Total > 0)
                            {
                                person.TotalDepositAccount = account.Total;
                            }
                            else
                            {
                                errorMessage.MessageShow("Максимальная сумма на счете 2100000000");
                                return;
                            }
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
