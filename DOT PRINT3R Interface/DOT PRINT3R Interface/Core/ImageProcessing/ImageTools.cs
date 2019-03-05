using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq.Expressions;

namespace Core.ImageProcessing
{
    static class ImageTools
    {
        public struct ImageConversionParams
        {
            public int Bias;
            public int QuantizeLevel;
            public Size ResizeSize;
            public bool NormalizeImage;
            public bool InvertImage;
        }

        public static void MakeGrayscale(this Image image)
        {
            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            Graphics graphics = Graphics.FromImage(bitmap);

            // Almost identity matrix, multiplied against the image to achieve good grayscale
            ColorMatrix grayscaleFilter = new ColorMatrix(
                new float[][] 
                {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1},
                }
            );

            ImageAttributes attribs = new ImageAttributes();
            attribs.SetColorMatrix(grayscaleFilter);

            graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attribs);

            graphics.Dispose();
        }

        public static byte[] MinifyBitmap(Bitmap image, ImageConversionParams p)
        {
            Bitmap newBitmap = new Bitmap(p.ResizeSize.Width, p.ResizeSize.Height);

            using (Graphics graphics = Graphics.FromImage(newBitmap))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                graphics.DrawImage(image, new Rectangle(0, 0, p.ResizeSize.Width, p.ResizeSize.Height));
            }

            float[,] brightnessMap = new float[newBitmap.Width, newBitmap.Height];

            // Get brightness map
            for (int i = 0; i < newBitmap.Height; i++)
                for (int j = 0; j < newBitmap.Width; j++)
                    brightnessMap[j, i] = newBitmap.GetPixel(j, i).GetBrightness();

            if (p.NormalizeImage)
                brightnessMap = NormalizeBrightnessMap(brightnessMap, newBitmap.Width, newBitmap.Height);

            for (int i = 0; i < newBitmap.Height; i++)
                for (int j = 0; j < newBitmap.Width; j++)
                    brightnessMap[j, i] += p.Bias;

            if (p.InvertImage)
                for (int i = 0; i < newBitmap.Height; i++)
                    for (int j = 0; j < newBitmap.Width; j++)
                        brightnessMap[j, i] = 1 - brightnessMap[j, i];

            byte[,] quantized = QuantizeBrightnessMap(brightnessMap, newBitmap.Width, newBitmap.Height, p.QuantizeLevel);

            byte[] result = new byte[newBitmap.Width * newBitmap.Height];
            for (int i = 0; i < newBitmap.Height; i++)
                for (int j = 0; j < newBitmap.Width; j++)
                    result[j + i * newBitmap.Width] = quantized[j, i];

            return result;

        }

        static float[,] NormalizeBrightnessMap(float[,] brightnessMap, int width, int height)
        {
            // Find min and max brightnesses
            float minBrightness = 1, maxBrightness = 0;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    minBrightness = minBrightness > brightnessMap[j, i] ? brightnessMap[j, i] : minBrightness;
                    minBrightness = maxBrightness < brightnessMap[j, i] ? brightnessMap[j, i] : maxBrightness;
                }

            // To prevent division by zero
            if (maxBrightness == 0) maxBrightness = .00001f;

            // Normalize image     
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    brightnessMap[j, i] = (brightnessMap[j, i] - minBrightness) / maxBrightness;

            return brightnessMap;
        }

        static byte[,] QuantizeBrightnessMap(float[,] brightnessMap, int width, int height, int quantizeLevel)
        {
            byte[,] result = new byte[width, height];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    result[j, i] = (byte)Math.Floor(brightnessMap[j, i] * quantizeLevel);

            return result;
        }
    }
}
