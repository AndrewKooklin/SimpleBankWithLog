using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using SimpleBank.Help;

namespace SimpleBank.Model
{
    [NotMapped]
    public class Client : Person
    {
        private SalaryAccount salaryAccount;
        private DepositAccount depositAccount;

        public Client( string lastName, string firstName, string fathersName, string phone, string passportNumber) : base( lastName, firstName, fathersName, phone, passportNumber)
        {
        }

        public SalaryAccount SalaryAccount
        {
            get { return salaryAccount; }
            set
            {
                if (value != salaryAccount)
                {
                    salaryAccount = value;
                    //OnPropertyChanged("SalaryAccount");
                }
            }
        }

        public DepositAccount DepositAccount 
        {
            get { return depositAccount; }
            set
            {
                if (value != depositAccount)
                {
                    depositAccount = value;
                    //OnPropertyChanged("DepositAccount");
                }
            }
        }   
    }
}
