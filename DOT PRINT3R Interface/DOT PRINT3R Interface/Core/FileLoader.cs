using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

using Lego.Ev3.Core;
using Lego.Ev3.Desktop;

namespace Core
{
    static class FileLoader
    {
        public static string filePath;

        public static void LoadImage()
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Image Files|*.png|*.jpg|*.jpeg"
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                filePath = dialog.FileName;
                Debug.WriteLine(string.Format("Loaded {0}", filePath));
            }
        }
    }
}
