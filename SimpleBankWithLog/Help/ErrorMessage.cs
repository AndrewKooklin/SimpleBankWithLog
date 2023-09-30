using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

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
            System.Windows.MessageBox.Show(text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public DialogResult MessageShowWithResult(string text)
        {
            DialogResult dialogResult = new DialogResult(); 

            dialogResult = System.Windows.Forms.MessageBox.Show(text, "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            return dialogResult;
        }
    }
}
