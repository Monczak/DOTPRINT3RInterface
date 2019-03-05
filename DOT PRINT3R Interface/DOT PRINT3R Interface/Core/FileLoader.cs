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

using Core.ImageProcessing;

namespace Core
{
    static class FileLoader
    {
        public static string filePath;

        public static ImageTools.ImageConversionParams p;

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

                ImageReader.LoadImage(filePath);

                p = new ImageTools.ImageConversionParams
                {
                    Bias = 0f,
                    QuantizeLevel = 4,
                    NormalizeImage = false,
                    ResizeSize = new System.Drawing.Size(28, 28),
                    InvertImage = false
                };

                ImageReader.ConvertImageToByteStream(p);
            }
        }
    }
}
