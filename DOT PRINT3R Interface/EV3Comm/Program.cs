using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;


namespace EV3Comm
{
    class Program
    {
        static int Main(string[] args)
        {
            Communication.ConnectToBrick();

            if (Communication.Connected)
            {
                try
                {
                    if (args == null || args.Length == 0) return -1;
                    string path = args[0];

                    if (Path.GetExtension(path) != ".rtf")
                    {
                        Console.WriteLine("E: File is invalid");
                        return -1;
                    }

                    Communication.SendFile(path).Wait();
                    Communication.TestConnection();
                }
                catch (Exception e) {
                    Console.WriteLine("E: Something went wrong: " + e.Message);
                    //Thread.Sleep(1000);
                    return -1;
                }

                Communication.Disconnect();
                return 0;
            }
            return -1;
        }
    }
}
