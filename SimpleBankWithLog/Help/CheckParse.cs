using System.Text.RegularExpressions;

namespace SimpleBank.Help
{
    /// <summary>
    /// Класс для проверки соответствия телефона и номера паспорта
    /// </summary>
    public class CheckParse
    {
        public CheckParse()
        {
        }

        public bool CheckParsePhone(string text)
        {
            text = text.Trim();
            Regex regex = new Regex(@"^\d{11}$");
            if (regex.IsMatch(text))
            {
                return true;
            }
            return false;
        }

        public bool CheckParsePassportNumber(string text)
        {
            text = text.Trim();

            Regex regex = new Regex(@"^\d{6}$");
            if (regex.IsMatch(text))
            {
                return true;
            }
            return false;
        }
    }
}
