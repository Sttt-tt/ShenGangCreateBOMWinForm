using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsApplication2
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Login login = new Login();
            DialogResult result = login.ShowDialog();
            if ((DialogResult.No == result) || (result == DialogResult.Cancel))
            {
                login.Dispose();
            }
            else
            {
                Application.Run(new Form1());
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}