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
    /// Команда перевода денег между собственными счетами клиента
    /// </summary>
    public class TransactionWithSelfAccountsCommand : ICommand
    {
        ObservableCollection<Person> _persons;
        Person person = new Person();
        int totalSalary;
        int totalDeposit;
        int newTotalSalary;
        int newTotalDeposit;
        bool convertTotalSalary;
        bool convertTotalDeposit;
        string stringQuery = "";
        SQLiteCommand SqliteCmd = new SQLiteCommand();

        public TransactionWithSelfAccountsCommand(ObservableCollection<Person> persons)
        {
            _persons = persons;
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
                bool checkId = Int32.TryParse(textBlockAccountId.Text, out int accountId);
                if (checkId)
                {
                    person = _persons.Single(p => p.PersonId == accountId);
                }
                else
                {
                    errorMessage.MessageShow("Некорректный Id");
                    return;
                }
                if (person.TotalSalaryAccount == null)
                {
                    errorMessage.MessageShow("Откройте зарплатный счет и внесите сумму");
                    return;
                }
                if (person.TotalDepositAccount == null)
                {
                    errorMessage.MessageShow("Откройте депозитный счет и внесите сумму");
                    return;
                }

                var comboBoxAccountTypeFrom = (ComboBox)childrenStackPanel[4];
                var chooseAccountFrom = (ComboBoxItem)comboBoxAccountTypeFrom.SelectedItem;
                if (chooseAccountFrom == null)
                {
                    errorMessage.MessageShow("Выберите тип счета списания");
                    return;
                }
                var comboBoxAccountTypeTo = (ComboBox)childrenStackPanel[6];
                var chooseAccountTo = (ComboBoxItem)comboBoxAccountTypeTo.SelectedItem;
                if (chooseAccountTo == null)
                {
                    errorMessage.MessageShow("Выберите тип счета зачисления");
                    return;
                }
                if (chooseAccountFrom.Content.Equals(chooseAccountTo.Content))
                {
                    errorMessage.MessageShow("Типы счетов совпадают,\n выберите разные счета.");
                    return;
                }

                var textBoxInputNumber = (TextBox)childrenStackPanel[8];

                bool parseTextBoxInputNumber = Int32.TryParse(textBoxInputNumber.Text, out int inputNumber);
                if(!parseTextBoxInputNumber || inputNumber > 2000000000)
                {
                    errorMessage.MessageShow("Введите сумму не более 2000000000");
                    return;
                }

                try
                {
                    SQLiteConnection connection = new SQLiteConnection(App.connectionString);
                    connection.Open();
                    
                    stringQuery = "SELECT TotalSalaryAccount FROM Persons WHERE PersonId=" + accountId + "";
                    
                    SqliteCmd.Connection = connection;
                    SqliteCmd.CommandText = stringQuery;
                    var resultTotalSalary = SqliteCmd.ExecuteScalar();

                    convertTotalSalary = Int32.TryParse(resultTotalSalary.ToString(), out totalSalary);

                    stringQuery = "SELECT TotalDepositAccount FROM Persons WHERE PersonId=" + accountId + "";
                    SqliteCmd.CommandText = stringQuery;
                    var resultTotalDeposit = SqliteCmd.ExecuteScalar();
                    connection.Close();

                    convertTotalDeposit = Int32.TryParse(resultTotalDeposit.ToString(), out totalDeposit);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    errorMessage.MessageShow("Не удалось подключиться к базе данных");
                }

                switch (chooseAccountFrom.Content.ToString())
                {
                    case "Зарплатный":
                        try
                        {
                            SQLiteConnection connection = new SQLiteConnection(App.connectionString);
                            connection.Open();
                            SqliteCmd.Connection = connection;

                            if (convertTotalSalary && parseTextBoxInputNumber)
                            {
                                newTotalSalary = totalSalary - inputNumber;
                                newTotalDeposit = totalDeposit + inputNumber;
                                if (newTotalSalary < 0)
                                {
                                    errorMessage.MessageShow("Введенная сумма больше остатка по счету списания");
                                    connection.Close();
                                    return;
                                }
                                else if(newTotalDeposit > 2100000000)
                                {
                                    errorMessage.MessageShow("Максимальная сумма на счете 2100000000");
                                    connection.Close();
                                    return;
                                }
                                person.TotalDepositAccount = newTotalDeposit;
                            }

                            if (newTotalSalary >= 0)
                            {
                                stringQuery = "UPDATE Persons SET TotalSalaryAccount=" + newTotalSalary + " WHERE PersonId=" + accountId + "";
                                SqliteCmd.CommandText = stringQuery;
                                SqliteCmd.ExecuteNonQuery();
                                stringQuery = "UPDATE Persons SET TotalDepositAccount=" + newTotalDeposit + " WHERE PersonId=" + accountId + "";
                                SqliteCmd.CommandText = stringQuery;
                                SqliteCmd.ExecuteNonQuery();
                                connection.Close();
                                person.TotalSalaryAccount = newTotalSalary;
                            }
                            else
                            {
                                errorMessage.MessageShow("Введенная сумма превышает остаток по счету списания");
                                connection.Close();
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
                    case "Депозитный":
                        try
                        {
                            SQLiteConnection connection = new SQLiteConnection(App.connectionString);
                            connection.Open();
                            SqliteCmd.Connection = connection;

                            if (convertTotalDeposit && parseTextBoxInputNumber)
                            {
                                newTotalDeposit = totalDeposit - inputNumber;
                                newTotalSalary = totalSalary + inputNumber;
                                
                                if (newTotalDeposit < 0)
                                {
                                    errorMessage.MessageShow("Введенная сумма больше остатка по счету списания");
                                    connection.Close();
                                    return;
                                }
                                else if (newTotalSalary > 2100000000)
                                {
                                    errorMessage.MessageShow("Максимальная сумма на счете 2100000000");
                                    connection.Close();
                                    return;
                                }
                                person.TotalSalaryAccount = newTotalSalary;
                            }

                            if (newTotalDeposit >= 0)
                            {
                                stringQuery = "UPDATE Persons SET TotalSalaryAccount=" + newTotalSalary + " WHERE PersonId=" + accountId + "";
                                SqliteCmd.CommandText = stringQuery;
                                SqliteCmd.ExecuteNonQuery();
                                stringQuery = "UPDATE Persons SET TotalDepositAccount=" + newTotalDeposit + " WHERE PersonId=" + accountId + "";
                                SqliteCmd.CommandText = stringQuery;
                                SqliteCmd.ExecuteNonQuery();
                                connection.Close();
                                person.TotalDepositAccount = newTotalDeposit;
                            }
                            else
                            {
                                errorMessage.MessageShow("Введенная сумма превышает остаток по счету списания");
                                connection.Close();
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
