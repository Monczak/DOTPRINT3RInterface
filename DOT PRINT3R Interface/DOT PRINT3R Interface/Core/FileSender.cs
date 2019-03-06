using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Core.ImageProcessing;

namespace Core
{
    static class FileSender
    {
        public static void ConnectAndSend()
        {
            EV3.ConnectToBrick();
            if (EV3.Connected)
            {
                Debug.WriteLine("Connected");
                EV3.TestConnection();
                EV3.SendFile(FileConverter.outputPath).Wait();
            }
            else {
                UIMessage.ShowError("Cannot connect to EV3 - make sure it's plugged in and turned on.");
            }
            //EV3.Disconnect();
            
        }
    }
}
