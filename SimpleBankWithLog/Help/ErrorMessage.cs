using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleBank.Help
{
    /// <summary>
    /// Класс для вывода сообщений об ошибках
    /// </summary>
    public class ErrorMessage
    {
        public ErrorMessage()
        {
        }

        public void MessageShow(string text)
        {
            MessageBox.Show(text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
