using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

            Thread thread = new Thread(ResetCommunicationFolders);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        static void ResetCommunicationFolders()
        {
            var _interfaceListener = new InterfaceListener();
            _interfaceListener.ResetCommunicationFolders();
        }
    }
}
