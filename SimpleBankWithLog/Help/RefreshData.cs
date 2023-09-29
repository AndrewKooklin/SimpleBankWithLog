using SimpleBank.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Help
{
    public class RefreshData
    {
        private ObservableCollection<Person> persons = new ObservableCollection<Person>();
        private ObservableCollection<UserOperation> userOperations = new ObservableCollection<UserOperation>();
        private GetDataFromDB getDataFromDB = new GetDataFromDB();

        public void RefreshDataPersons()
        {
            persons = getDataFromDB.GEtAllPersonsFromDB();
            App.mainWindow.lbPersonsItems.ItemsSource = persons;
            App.mainWindow.lbPersonsItems.Items.Refresh();
        }

        public void RefreshDataToUserOptionsWindow()
        {
            userOperations = getDataFromDB.GetUserOperationsFromDB();
            App.recordOperationsWindow.lbOperations.ItemsSource = userOperations;
            App.recordOperationsWindow.lbOperations.Items.Refresh();
        }
    }
}
