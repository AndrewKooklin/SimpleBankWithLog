using SimpleBank.Command;
using SimpleBank.Data;
using SimpleBank.Model;

namespace SimpleBank.ViewModel
{
    /// <summary>
    /// Вью модель клиента банка
    /// </summary>
    public class PersonViewModel : ViewModelBase
    {
        private readonly Person _person;
        private readonly MainWindowViewModel _mainWindowViewModel;

        public int PersonId => _person.PersonId;

        public string LastName => _person.LastName;

        public string FirstName => _person.FirstName;

        public string FathersName => _person.FathersName;

        public string Phone => _person.Phone;

        public string PassportNumber => _person.PassportNumber;

        public PersonViewModel()
        {
            
        }    

        public PersonViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }   
    }
}
