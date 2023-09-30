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
    /// Команда перевода денег между счетами клиентов
    /// </summary>
    public class TransactionBetweenClientsCommand : ICommand
    {
        ObservableCollection<Person> _persons;
        Person personSend = new Person();
        Person personRecieve = new Person();
        private SalaryAccount salaryAccountFrom = new SalaryAccount();
        private SalaryAccount salaryAccountTo = new SalaryAccount();
        private DepositAccount depositAccountFrom = new DepositAccount();
        private DepositAccount depositAccountTo = new DepositAccount();
        private Account account = new Account();
        private Account accountFrom = new Account();
        private Account accountTo = new Account();
        private TransactionBetweenClients tbc = new TransactionBetweenClients();
        int totalFrom;
        int totalTo;
        bool convertTotalFrom;
        bool convertTotalTo;
        string totalAccountFrom = "";
        string totalAccountTo = "";
        string stringQuery = "";
        SQLiteCommand SqliteCmd = new SQLiteCommand();
        private event Action<string, string, int?> RecordOperation;
        private event Action RefreshListOperations;

        public TransactionBetweenClientsCommand(ObservableCollection<Person> persons)
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

                var textBlockAccountIdFrom = (TextBlock)childrenStackPanel[1];
                if (String.IsNullOrWhiteSpace(textBlockAccountIdFrom.Text))
                {
                    errorMessage.MessageShow("Выберите клиента отправителя");
                    return;
                }
                bool checkIdFrom = Int32.TryParse(textBlockAccountIdFrom.Text, out int accountIdFrom);
                if (checkIdFrom)
                {
                    personSend = _persons.Single(p => p.PersonId == accountIdFrom);
                }
                else
                {
                    errorMessage.MessageShow("Некорректный Id");
                    return;
                }

                var textBlockAccountIdTo = (TextBlock)childrenStackPanel[6];
                if (String.IsNullOrWhiteSpace(textBlockAccountIdTo.Text))
                {
                    errorMessage.MessageShow("Выберите клиента получателя");
                    return;
                }

                bool checkIdTo = Int32.TryParse(textBlockAccountIdTo.Text, out int accountIdTo);
                if (checkIdTo)
                {
                    personRecieve = _persons.Single(p => p.PersonId == accountIdTo);
                }
                else
                {
                    errorMessage.MessageShow("Некорректный Id");
                    return;
                }

                var comboBoxAccountTypeFrom = (ComboBox)childrenStackPanel[4];
                var chooseAccountFrom = (ComboBoxItem)comboBoxAccountTypeFrom.SelectedItem;
                if (chooseAccountFrom == null)
                {
                    errorMessage.MessageShow("Выберите тип счета списания");
                    return;
                }
                if (chooseAccountFrom.Content.Equals("Зарплатный"))
                {
                    totalAccountFrom = "TotalSalaryAccount";
                    accountFrom = salaryAccountFrom;
                    if (personSend.TotalSalaryAccount == null)
                    {
                        errorMessage.MessageShow("Откройте зарплатный счет отправителя и внесите сумму");
                        return;
                    }
                }
                else if (chooseAccountFrom.Content.Equals("Депозитный"))
                {
                    totalAccountFrom = "TotalDepositAccount";
                    accountFrom = depositAccountFrom;
                    if (personSend.TotalDepositAccount == null)
                    {
                        errorMessage.MessageShow("Откройте депозитный счет отправителя и внесите сумму");
                        return;
                    }
                }
                var comboBoxAccountTypeTo = (ComboBox)childrenStackPanel[9];
                var chooseAccountTo = (ComboBoxItem)comboBoxAccountTypeTo.SelectedItem;
                if (chooseAccountTo == null)
                {
                    errorMessage.MessageShow("Выберите тип счета зачисления");
                    return;
                }
                if (chooseAccountTo.Content.Equals("Зарплатный"))
                {
                    totalAccountTo = "TotalSalaryAccount";
                    accountTo = salaryAccountTo;
                    if (personRecieve.TotalSalaryAccount == null)
                    {
                        errorMessage.MessageShow("Откройте зарплатный счет получателя и внесите сумму");
                        return;
                    }
                }
                else if (chooseAccountTo.Content.Equals("Депозитный"))
                {
                    totalAccountTo = "TotalDepositAccount";
                    accountTo = depositAccountTo;
                    if (personRecieve.TotalDepositAccount == null)
                    {
                        errorMessage.MessageShow("Откройте депозитный счет получателя и внесите сумму");
                        return;
                    }
                }

                var textBoxInputNumber = (TextBox)childrenStackPanel[11];

                bool parseTextBoxInputNumber = Int32.TryParse(textBoxInputNumber.Text, out int inputNumber);
                if (!parseTextBoxInputNumber || inputNumber > 2000000000)
                {
                    errorMessage.MessageShow("Введите сумму не более 2000000000");
                    return;
                }

                try
                {
                    SQLiteConnection connection = new SQLiteConnection(App.connectionString);
                    connection.Open();

                    stringQuery = "SELECT " + totalAccountFrom + " FROM Persons WHERE PersonId=" + accountIdFrom + "";

                    SqliteCmd.Connection = connection;
                    SqliteCmd.CommandText = stringQuery;
                    var resultTotalFrom = SqliteCmd.ExecuteScalar();

                    convertTotalFrom = Int32.TryParse(resultTotalFrom.ToString(), out totalFrom);
                    accountFrom.Total = totalFrom;

                    stringQuery = "SELECT " + totalAccountTo + " FROM Persons WHERE PersonId=" + accountIdTo + "";
                    SqliteCmd.CommandText = stringQuery;
                    var resultTotalTo = SqliteCmd.ExecuteScalar();
                    connection.Close();

                    convertTotalTo = Int32.TryParse(resultTotalTo.ToString(), out totalTo);
                    accountTo.Total = totalTo;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    errorMessage.MessageShow("Не удалось подключиться к базе данных");
                }


                try
                {
                    SQLiteConnection connection = new SQLiteConnection(App.connectionString);
                    connection.Open();
                    SqliteCmd.Connection = connection;

                    if (convertTotalFrom && convertTotalTo && parseTextBoxInputNumber)
                    {
                        tbc.Transact(accountFrom, accountTo, inputNumber);

                        if (accountFrom.Total < 0)
                        {
                            errorMessage.MessageShow("Введенная сумма больше остатка по счету списания");
                            connection.Close();
                            return;
                        }
                        else if (accountTo.Total > 2100000000)
                        {
                            errorMessage.MessageShow("Максимальная сумма на счете 2100000000");
                            connection.Close();
                            return;
                        }
                        
                        if (chooseAccountTo.Content.Equals("Зарплатный"))
                        {
                            personRecieve.TotalSalaryAccount = accountTo.Total;
                        }
                        else if (chooseAccountTo.Content.Equals("Депозитный"))
                        {
                            personRecieve.TotalDepositAccount = accountTo.Total;
                        }  
                    }

                    if (accountFrom.Total >= 0)
                    {
                        stringQuery = "UPDATE Persons SET "+totalAccountFrom+"=" + accountFrom.Total + " WHERE PersonId=" + accountIdFrom + "";
                        SqliteCmd.CommandText = stringQuery;
                        SqliteCmd.ExecuteNonQuery();
                        stringQuery = "UPDATE Persons SET " + totalAccountTo + "=" + accountTo.Total + " WHERE PersonId=" + accountIdTo + "";
                        SqliteCmd.CommandText = stringQuery;
                        SqliteCmd.ExecuteNonQuery();
                        connection.Close();

                        if (chooseAccountFrom.Content.Equals("Зарплатный"))
                        {
                            personSend.TotalSalaryAccount = accountFrom.Total;
                        }
                        else if (chooseAccountFrom.Content.Equals("Депозитный"))
                        {
                            personSend.TotalDepositAccount = accountFrom.Total;
                        }
                    }
                    else
                    {
                        errorMessage.MessageShow("Введенная сумма превышает остаток по счету списания");
                        connection.Close();
                        return;
                    }

                    string info = "Перевод денег со счета " + "\"" + chooseAccountFrom.Content.ToString() + "\""
                                            + " клиента : " + App.abbreviatedName.GetFIO(personSend) 
                                            + " \nна счет " + "\"" + chooseAccountTo.Content.ToString() + "\""
                                            + " клиента : " + App.abbreviatedName.GetFIO(personRecieve);

                    RecordOperation?.Invoke(App.mainWindow.Title, info, inputNumber);
                    App.refreshData.RefreshDataPersons();
                    RefreshListOperations?.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    errorMessage.MessageShow("Не удалось подключиться к базе данных");
                }
            }
        }
    }
}
