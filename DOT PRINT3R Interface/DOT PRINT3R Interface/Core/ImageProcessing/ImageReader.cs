using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace Core.ImageProcessing
{
    static class ImageReader
    {
        public static byte[] stream;

        private static Bitmap bitmap;

        public static void LoadImage(string path)
        {
            bitmap = new Bitmap(Image.FromFile(path));
        }

        public static void ConvertImageToByteStream(ImageTools.ImageConversionParams p)
        {
            stream = ImageTools.MinifyBitmap(bitmap, p);

            for (int i = 0; i < p.ResizeSize.Height; i++)
            {
                string line = "";
                for (int j = 0; j < p.ResizeSize.Width; j++)
                {
                    line += stream[j + i * p.ResizeSize.Width].ToString();
                }
                Debug.WriteLine(line);
            }
        }

    
    }
}
