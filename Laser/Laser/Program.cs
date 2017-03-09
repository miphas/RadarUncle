using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Laser {
    static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new Form1();
            Application.Run(form);
        }

        public static Form1 form;
    }
}
