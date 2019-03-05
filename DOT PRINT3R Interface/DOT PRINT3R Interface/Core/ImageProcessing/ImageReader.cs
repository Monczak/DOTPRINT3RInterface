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
        }

    
    }
}
