using SimpleBank.Data;
using SimpleBank.Model;
using SimpleBank.View;
using SimpleBank.ViewModel;
using System;
using System.Windows.Input;

namespace SimpleBank.Commands
{
    public class OpenListOperationsCommand : ICommand
    {
        private RecordOperationsWindowViewModel recordOperationsWindowViewModel;
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
            App.recordOperationsWindow = new RecordOperationsWindow();
            recordOperationsWindowViewModel = new RecordOperationsWindowViewModel();
            App.recordOperationsWindow.Show();
        }
    }
}
