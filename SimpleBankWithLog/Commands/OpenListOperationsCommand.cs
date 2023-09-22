using SimpleBank.Data;
using SimpleBank.Model;
using SimpleBank.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleBank.Commands
{
    public class OpenListOperationsCommand : ICommand
    {
        private RecordOperationsWindow recordOperationsWindow;
        private SimpleBankContext _db;

        public OpenListOperationsCommand(SimpleBankContext db)
        {
            this._db = db;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            recordOperationsWindow = new RecordOperationsWindow();

            recordOperationsWindow.lbPersonsItems.ItemsSource = GEtAllOperations(_db);
            recordOperationsWindow.Show();
        }

        private ObservableCollection<UserOperation> GEtAllOperations(SimpleBankContext db)
        {
            ObservableCollection<UserOperation> _operations = new ObservableCollection<UserOperation>();

            try
            {
                using (db = new SimpleBankContext())
                {
                    foreach (var operation in db.UserOperations.AsEnumerable())
                    {
                        _operations.Add(operation);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return _operations;
        }
    }
}
