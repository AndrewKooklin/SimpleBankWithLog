using SimpleBank.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Help
{
    public class GetAbbreviatedName
    {
        public string GetFIO(Person person)
        {
            string fio = "";

            string firstLetterFirstName = person.FirstName.ToUpper()
                                                                  .Substring(0, 1);
            string firstLetterFathersName = person.FathersName.ToUpper()
                                                              .Substring(0, 1);

            fio = person.LastName + " " + firstLetterFirstName + "." + firstLetterFathersName + ".";

            return fio;
        }
    }
}
