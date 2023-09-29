using SimpleBank.Data;
using SimpleBank.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Help
{
    public class RecordOperation
    {
        SQLiteCommand SqliteCmd = new SQLiteCommand();
        ErrorMessage errorMessage = new ErrorMessage();

        public RecordOperation()
        {
        }

        public void RecordOperationToBD(string role, string operation, int? totalSum)
        {
            try
            {
                SQLiteConnection connection = new SQLiteConnection(App.connectionString);
                connection.Open();

                string dateTimeOperation = DateTime.Now.ToLocalTime().ToString();

                string stringQuery =  $"INSERT INTO UserOperations(Role, DataOperation, Operation, TotalSum) VALUES ('{role}', '{dateTimeOperation}', '{operation}', '{totalSum}')";

                SqliteCmd.Connection = connection;
                SqliteCmd.CommandText = stringQuery;
                var resultTotalSalary = SqliteCmd.ExecuteScalar();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                errorMessage.MessageShow("Не удалось подключиться к базе данных");
            }
        }

        public void RefreshListOperations()
        {
            
        }
    }
}
