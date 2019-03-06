using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using Core.ImageProcessing;

namespace Core
{
    static class FileConverter
    {
        public static string outputPath;

        public static void ConvertFile(string path)
        {
            string file = string.Format("{0}\r{1}\r", FileLoader.p.ResizeSize.Width, FileLoader.p.ResizeSize.Height);
            foreach (byte b in ImageReader.stream)
            {
                file += b.ToString() + '\r';
            }
            file += "99\r";
            File.WriteAllText(path, file);
            outputPath = path;
        }
    }
}
