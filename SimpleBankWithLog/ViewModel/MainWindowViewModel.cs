using SimpleBank.Command;
using SimpleBank.Commands;
using SimpleBank.Data;
using SimpleBank.Help;
using SimpleBank.Model;
using SimpleBank.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleBank.ViewModel
{
    /// <summary>
    /// Вью модель главного окна
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        public string NamePage
        {
            get { return "Главная"; }
        }

        private SimpleBankContext _db;
        private MainWindowViewModel _mainWindowViewModel;
        private MainWindow _mainWindow;
        private Person _selectedPerson;
        private int _selectedCount = 0;
        private ObservableCollection<Person> _persons;
        private ObservableCollection<UserOperation> _userOperations;
        private GetDataFromDB getDataFromDB = new GetDataFromDB();

        public ObservableCollection<Person> Persons
        {
            get { return _persons; }
            set
            {
                _persons = value;
                OnPropertyChanged(nameof(Persons));
            }
        }

        public ObservableCollection<UserOperation> UserOperations
        {
            get { return _userOperations; }
            set
            {
                _userOperations = value;
                OnPropertyChanged(nameof(UserOperations));
            }
        }


        //private ViewModelBase _leftCurrentViewModel;
        private UserControl _rightCurrentView;

        //public ViewModelBase LeftCurrentViewModel
        //{
        //    get
        //    { 
        //        return _leftCurrentViewModel;
        //    }
        //    set
        //    {
        //        _leftCurrentViewModel = value;
        //        OnPropertyChanged(nameof(LeftCurrentViewModel));
        //    }
        //}

        public UserControl RightCurrentView
        {
            get { return _rightCurrentView; }
            set 
            { 
                _rightCurrentView = value;
                OnPropertyChanged(nameof(RightCurrentView));
            }
        }

        public ICommand UpdateViewCommand { get; set; }

        public ICommand GetClientsCommand { get; set; }

        public ICommand CreatePersonCommand { get; set; }

        public ICommand ChangePersonCommand { get; set; }

        public ICommand DeletePersonCommand { get; set; }

        public ICommand ClearPersonCommand { get; set; }

        public ICommand OpenAccountCommand { get; set; }

        public ICommand CloseAccountCommand { get; set; }

        public ICommand PutMoneyCommand { get; set; }

        public ICommand WithdrawMoneyCommand { get; set; }

        public ICommand TransactionWithSelfAccountsCommand { get; set; }

        public ICommand TransactionBetweenClientsCommand { get; set; }

        public ICommand OpenListOperationsCommand { get; set; }

        public ICommand SelectUserCommand { get; set; }

        public int? SelectedIndexPerson { get; set; }

        public Person SelectedItemPerson { get; set; }

        public Person SelectedPerson 
        {
            get 
            {
                if (_selectedPerson == null)
                {
                    SelectedIndexPerson = null;

                    return null;
                }
                if (RightCurrentView == null)
                {
                    return null;
                }
                
                else if(RightCurrentView is PersonView)
                {
                    SelectedIndexPerson = App.mainWindow.lbPersonsItems.SelectedIndex;

                    var view = (PersonView)RightCurrentView;
                    //SelectedIndexPerson = _selectedPerson.PersonId;
                    view.tbSelectedIndexPerson.Text = SelectedIndexPerson.ToString();
                    view.tbPersonId.Text = _selectedPerson.PersonId.ToString();
                    view.tbLastName.Text = _selectedPerson.LastName;
                    view.tbFirstName.Text = _selectedPerson.FirstName;
                    view.tbFathersName.Text = _selectedPerson.FathersName;
                    view.tbPhone.Text = _selectedPerson.Phone;
                    view.tbPassportNumber.Text = _selectedPerson.PassportNumber;
                }

                else if (RightCurrentView is AccountActionView)
                {
                    var view = (AccountActionView)RightCurrentView;
                    
                    if (_selectedPerson != null) 
                    {  
                        string firstLetterFirstName = _selectedPerson.FirstName
                                                                    .ToUpper()
                                                                    .Substring(0,1);
                        string firstLetterFathersName = _selectedPerson.FathersName
                                                                      .ToUpper()
                                                                      .Substring(0, 1);

                        view.tbFIO.Text  = _selectedPerson.LastName + " "
                                        + firstLetterFirstName + "." 
                                        + firstLetterFathersName + ".";

                        view.tbAccountId.Text = _selectedPerson.PersonId.ToString();
                    }
                }

                else if (RightCurrentView is TransactionWithSelfView)
                {
                    var view = (TransactionWithSelfView)RightCurrentView;

                    if (_selectedPerson != null)
                    {
                        string firstLetterFirstName = _selectedPerson.FirstName
                                                                    .ToUpper()
                                                                    .Substring(0, 1);
                        string firstLetterFathersName = _selectedPerson.FathersName
                                                                      .ToUpper()
                                                                      .Substring(0, 1);

                        view.tbFIO.Text = _selectedPerson.LastName + " "
                                        + firstLetterFirstName + "."
                                        + firstLetterFathersName + ".";

                        view.tbAccountId.Text = _selectedPerson.PersonId.ToString();
                    }
                }

                else if (RightCurrentView is TransactionBetweenClientsView)
                {
                    var view = (TransactionBetweenClientsView)RightCurrentView;

                    if (_selectedPerson != null && _selectedCount == 0)
                    {
                        string firstLetterFirstName = _selectedPerson.FirstName
                                                                    .ToUpper()
                                                                    .Substring(0, 1);
                        string firstLetterFathersName = _selectedPerson.FathersName
                                                                      .ToUpper()
                                                                      .Substring(0, 1);

                        view.tbFIOFrom.Text = _selectedPerson.LastName + " "
                                        + firstLetterFirstName + "."
                                        + firstLetterFathersName + ".";

                        view.tbAccountIdFrom.Text = _selectedPerson.PersonId.ToString();

                        _selectedCount++;
                    }
                    else if(_selectedPerson != null && _selectedCount == 1)
                    {
                        string firstLetterFirstName = _selectedPerson.FirstName
                                                                    .ToUpper()
                                                                    .Substring(0, 1);
                        string firstLetterFathersName = _selectedPerson.FathersName
                                                                      .ToUpper()
                                                                      .Substring(0, 1);

                        view.tbFIOTo.Text = _selectedPerson.LastName + " "
                                        + firstLetterFirstName + "."
                                        + firstLetterFathersName + ".";

                        view.tbAccountIdTo.Text = _selectedPerson.PersonId.ToString();

                        _selectedCount = 0;
                    }
                }

                return _selectedPerson;
            }
            set
            {
                _selectedPerson = new Person();
                _selectedPerson = value;
                OnPropertyChanged(nameof(SelectedPerson));
            }
        }

        public MainWindowViewModel()
        {
            UpdateViewCommand = new UpdateViewCommand(_db, this);

            _mainWindowViewModel = this;

            Persons = new ObservableCollection<Person>();

            Persons = getDataFromDB.GEtAllPersonsFromDB();

            CreatePersonCommand = new CreatePersonCommand(_db,
                                                          Persons,
                                                          _mainWindowViewModel,
                                                          _mainWindow);

            ChangePersonCommand = new ChangePersonCommand(_db,
                                                          Persons,
                                                          _mainWindowViewModel,
                                                          _mainWindow);

            DeletePersonCommand = new DeletePersonCommand(_db,
                                                          Persons,
                                                          _mainWindowViewModel,
                                                          _mainWindow);

            ClearPersonCommand = new ClearPersonCommand();

            OpenAccountCommand = new OpenAccountCommand(Persons);

            CloseAccountCommand = new CloseAccountCommand(Persons);

            PutMoneyCommand = new PutMoneyCommand(Persons);

            WithdrawMoneyCommand = new WithdrawMoneyCommand(Persons);

            TransactionWithSelfAccountsCommand = new TransactionWithSelfAccountsCommand(Persons);

            TransactionBetweenClientsCommand = new TransactionBetweenClientsCommand(Persons);

            OpenListOperationsCommand = new OpenListOperationsCommand(_db);

            SelectUserCommand = new SelectUserCommand();
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
