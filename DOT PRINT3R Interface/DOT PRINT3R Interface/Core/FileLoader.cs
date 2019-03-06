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

        public enum ImageLoadResult
        {
            Success,
            NotPicked,
            Invalid
        }

        public static ImageLoadResult LoadImage()
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.gif|All Files|*.*"
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                filePath = dialog.FileName;
                Debug.WriteLine(string.Format("Loaded {0}", filePath));

                try
                {
                    ImageReader.LoadImage(filePath);
                }
                catch (OutOfMemoryException)
                {
                    return ImageLoadResult.Invalid;
                }
                return ImageLoadResult.Success;
            }

            return ImageLoadResult.NotPicked;
        }
    }
}
