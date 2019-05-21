using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;  //ONLY FOR TEST

namespace AirportsTracking
{
    static class Program
    {
        [DllImport("Kernel32.dll")] //ONLY FOR TEST
        static extern Boolean AllocConsole(); //ONLY FOR TEST
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
