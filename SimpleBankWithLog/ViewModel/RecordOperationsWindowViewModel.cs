using SimpleBank.Data;
using SimpleBank.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.ViewModel
{
    public class RecordOperationsWindowViewModel : ViewModelBase
    {
        private SimpleBankContext db;

        private ObservableCollection<UserOperation> _userOperations;

        public ObservableCollection<UserOperation> UserOperations
        {
            get { return _userOperations; }
            set
            {
                _userOperations = value;
                OnPropertyChanged(nameof(UserOperations));
            }
        }

        public RecordOperationsWindowViewModel(SimpleBankContext _db)
        {
            UserOperations = new ObservableCollection<UserOperation>();
            db = _db;
            UserOperations = GEtAllOperations(db);
        }

        public RecordOperationsWindowViewModel()
        {
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
