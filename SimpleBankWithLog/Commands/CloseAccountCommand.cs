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
    /// Команда удаления счета
    /// </summary>
    public class CloseAccountCommand : ICommand
    {
        ObservableCollection<Person> _persons;
        Person person = new Person();
        private string AccountType = "";

        public CloseAccountCommand(ObservableCollection<Person> persons)
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
                var comboBoxAccountType = (ComboBox)childrenStackPanel[4];
                var choose = (ComboBoxItem)comboBoxAccountType.SelectedItem;
                if (choose == null)
                {
                    errorMessage.MessageShow("Выберите тип счета");
                    return;
                }
                if (choose.Content.Equals("Зарплатный"))
                {
                    AccountType = "TotalSalaryAccount";
                }
                else if (choose.Content.Equals("Депозитный"))
                {
                    AccountType = "TotalDepositAccount";
                }

                try
                {
                    string connecionString = @"Data Source=C:\repos\SimpleBank\SimpleBank\Data\SimpleBank.db;New=False;Compress=True;";
                    SQLiteConnection connection = new SQLiteConnection(connecionString);
                    connection.Open();
                    string stringQuery = "";
                    bool checkId = Int32.TryParse(textBlockAccountId.Text, out int AccountId);
                    if (checkId)
                    {
                        stringQuery = "SELECT " + AccountType + " FROM Persons WHERE PersonId=" + AccountId + "";
                    }
                    else
                    {
                        errorMessage.MessageShow("Некорректный Id");
                        return;
                    }
                    var SqliteCmd = new SQLiteCommand();
                    SqliteCmd.Connection = connection;
                    SqliteCmd.CommandText = stringQuery;
                    var result = SqliteCmd.ExecuteScalar();

                    bool convertTotal = Int32.TryParse(result.ToString(), out int total);
                    if (convertTotal && total > 0)
                    {
                        errorMessage.MessageShow("Для закрытия снимите все деньги со счета");
                        connection.Close();
                        return;
                    }
                    if (convertTotal && total == 0)
                    {
                        stringQuery = "UPDATE Persons SET " + AccountType + "=NULL WHERE PersonId=" + AccountId + "";
                        SqliteCmd.CommandText = stringQuery;
                        SqliteCmd.ExecuteScalar();
                    }
                    
                    connection.Close();

                    person = _persons.Single(p => p.PersonId == AccountId);
                    if (choose.Content.Equals("Зарплатный") && person.TotalSalaryAccount == 0)
                    {
                        person.TotalSalaryAccount = null;
                    }
                    if(choose.Content.Equals("Депозитный") && person.TotalDepositAccount == 0)
                    {
                        person.TotalDepositAccount = null;
                    }
                    AccountType = "";

                    App.mainWindow.lbPersonsItems.ItemsSource = _persons;
                    App.mainWindow.lbPersonsItems.Items.Refresh();
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
