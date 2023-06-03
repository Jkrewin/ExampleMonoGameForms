using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonoGame.Forms.DX
{
    internal static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            string pathDirB = Application.StartupPath + @"\Content\";
            string pathDirA = Application.StartupPath + @"\..\..\Content\bin\Windows\";

            if (Directory.Exists (pathDirB)==false)  Directory.CreateDirectory(pathDirB);
            foreach (string dirPath in Directory.GetDirectories(pathDirA, "*", SearchOption.AllDirectories))
            {
                string dir = dirPath.Replace(pathDirA, pathDirB);
                if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);
            }
            foreach (string filePath in Directory.GetFiles(pathDirA, "*.*", SearchOption.AllDirectories))
            {
                string file = filePath.Replace(pathDirA, pathDirB);
                File.Copy(filePath, file, true);
            }        
#endif

        Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
