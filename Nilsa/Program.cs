using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nilsa.NilsaAndInterface;

namespace Nilsa
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
            var _interfaceListener = new InterfaceListener();
            _interfaceListener.ResetCommunicationFoulders();//очистили папки с общением с интерфейсом после прошлого запуска
        }
    }
}
