﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
        }

        static void ResetCommunicationFolders()
        {
            var _interfaceListener = new InterfaceListener();
            _interfaceListener.ResetCommunicationFolders();
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            // Обработка исключения
            MessageBox.Show("Произошло необработанное исключение: " + e.Exception.Message);
        }
    }
}
