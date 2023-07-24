using System;

namespace SimpleBank.Model
{
    /// <summary>
    /// Вспомогательный класс перевода денег между счетами клиентов
    /// </summary>
    public class TransactionBetweenClients : IAccountContrvariant<Account, Account>
    {
        public void Transact(Account accountFrom, Account accountTo, int sum)
        {
            accountFrom.Total -= sum;
            accountTo.Total += sum;
        }
    }
}
