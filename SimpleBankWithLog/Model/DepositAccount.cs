using SQLite.CodeFirst;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleBank.Model
{
    /// <summary>
    /// Класс депозитного счета
    /// </summary>
    [Table("DepositAccounts")]
    public class DepositAccount : Account, IAccountCovariant<Account>
    {
        public DepositAccount()
        {
        }

        public DepositAccount(int total)
        {
            Total = 0;
            DateDepositOpen = DateTime.Now;
        }

        //[Key,Autoincrement]
        //[Unique]
        //[Required]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("DepositAccountId")]
        public int DepositAccountId { get; set; }

        [Column("DateDepositOpen")]
        public DateTime DateDepositOpen { get; set; }

        public Account PutMoney(Account account, int sum)
        {
            account.Total += sum;
            return account;
        }
    }
}
