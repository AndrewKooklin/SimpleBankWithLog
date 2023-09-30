using SimpleBank.Data;
using SimpleBank.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Help
{
    public class GetDataFromDB
    {
        private ObservableCollection<UserOperation> userOperations;
        string stringQuery = "";
        SQLiteCommand SqliteCmd = new SQLiteCommand();
        private SimpleBankContext _db;

        public ObservableCollection<Person> GEtAllPersonsFromDB()
        {
            ObservableCollection<Person> _persons = new ObservableCollection<Person>();

            try
            {

                using (_db = new SimpleBankContext())
                {
                    foreach (var person in _db.Persons.AsEnumerable())
                    {
                        _persons.Add(person);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return _persons;
        }

        public ObservableCollection<UserOperation> GetUserOperationsFromDB()
        {
            userOperations = new ObservableCollection<UserOperation>();

            int? totalSum;

            try
            {
                SQLiteConnection connection = new SQLiteConnection(App.connectionString);
                connection.Open();

                stringQuery = "SELECT Role, DataOperation, Operation, TotalSum FROM UserOperations";

                SqliteCmd.Connection = connection;
                SqliteCmd.CommandText = stringQuery;
                SQLiteDataReader dataReader = SqliteCmd.ExecuteReader();

                while (dataReader.Read())
                {
                    object role = dataReader["Role"];
                    object dataOperation = dataReader["DataOperation"];
                    object operation = dataReader["Operation"];
                    object totalSumRead = dataReader["TotalSum"];
                    if(totalSumRead.ToString().Equals("0") || totalSumRead == null)
                    {
                        totalSum = null;
                    }
                    else totalSum = System.Int32.Parse(totalSumRead.ToString());

                    UserOperation userOperation = new UserOperation(role.ToString(), dataOperation.ToString(),
                                                                    operation.ToString(), totalSum);

                    userOperations.Add(userOperation);
                }

                dataReader.Close();
                connection.Close();

                return userOperations;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return userOperations;
        }
    }
}
