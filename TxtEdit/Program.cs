using Associations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TxtEdit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            /*foreach (var arg in args)
            {
                MessageBox.Show(arg);
            }*/
            if (args.Length != 0)
            {
                Editor2.CurrentFile = args[0];
                //MessageBox.Show(Editor2.CurrentFile);
            }
            else
            {
                
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Editor2());

        }

    }
}
