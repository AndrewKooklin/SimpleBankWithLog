using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Model
{
    /// <summary>
    /// Контрвариантный интерфейс перевода денег меджу счетами клиентов
    /// </summary>
    public interface IAccountContrvariant<in T1,T2>
    {
        void Transact(Account T1, Account T2, int sum);
    }
}
