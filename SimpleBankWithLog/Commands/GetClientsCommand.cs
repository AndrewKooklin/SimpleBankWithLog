using SimpleBank.Data;
using SimpleBank.Model;
using SimpleBank.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleBank.Commands
{
    /// <summary>
    /// Команда получения списка клиентов
    /// </summary>
    public class GetClientsCommand : ICommand
    {
        private MainWindowViewModel _mainWindowViewModel;
        private SimpleBankContext _db;

        public GetClientsCommand(SimpleBankContext simpleBankContext,
                                MainWindowViewModel mainWindowViewModel)
        {
            _db = simpleBankContext;
            _mainWindowViewModel = mainWindowViewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _mainWindowViewModel.Persons = GEtAllPersons(_db);
        }

        private ObservableCollection<Person> GEtAllPersons(SimpleBankContext db)
        {
            ObservableCollection<Person> _persons = new ObservableCollection<Person>();

            try
            {
                using (db = new SimpleBankContext())
                {
                    foreach (var person in db.Persons.AsEnumerable())
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
    }
}
