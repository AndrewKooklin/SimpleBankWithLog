using SQLite.CodeFirst;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleBank.Model
{
    /// <summary>
    /// Модель зарплатного счета
    /// </summary>
    [Table("SalaryAccounts")]
    public class SalaryAccount : Account, IAccountCovariant<Account>
    {
        public SalaryAccount()
        {
        }

        public SalaryAccount(int total)
        {
            Total = 0;
            DateSalaryOpen = DateTime.Now;
        }

        //[Key,Autoincrement]
        //[Unique]
        //[Required]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("SalaryAccountId")]
        public int SalaryAccountId { get; set; }

        [Column("DateSalaryOpen")]
        public DateTime DateSalaryOpen { get; set; }

        public Account PutMoney(Account account, int sum)
        {
            account.Total += sum;
            return account;
        }
    }
}
