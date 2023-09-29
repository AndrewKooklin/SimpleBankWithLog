using SimpleBank.Data;
using SimpleBank.Help;
using SimpleBank.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;

namespace SimpleBank.ViewModel
{
    public class RecordOperationsWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private ObservableCollection<UserOperation> _operations;
        private GetDataFromDB getDataFromDb = new GetDataFromDB();

        public RecordOperationsWindowViewModel()
        {
            Operations = getDataFromDb.GetUserOperationsFromDB();
        }

        public ObservableCollection<UserOperation> Operations
        {
            get { return _operations; }
            set
            {
                _operations = value;
                OnPropertyChanged(nameof(Operations));
            }
        }
    }
}
