using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using Lego.Ev3.Core;
using Lego.Ev3.Desktop;

namespace Core
{
    static class EV3
    {
        public static Brick brick;

        public static bool Connected = false;

        public static async void ConnectToBrick()
        {
            brick = new Brick(new UsbCommunication(), true);
            Connected = true;
            try
            {
                await brick.ConnectAsync();
                Debug.WriteLine("I: Connected");
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("E: Cannot connect to EV3: {0}", e.Message));
                Connected = false;
            }
        }

        public static async void TestConnection()
        {
            await brick.DirectCommand.PlayToneAsync(50, 1000, 500);
        }

        public static void Disconnect()
        {
            brick.Disconnect();
            Connected = false;
            Debug.WriteLine("I: Disconnected");
        }

        public static async Task SendFile(string path)
        {
            byte[] data = File.ReadAllBytes(path);

            // Replace line endings
            for (int i = 0; i < data.Length; i++)
                if (data[i] == 10) data[i] = 13;

            await brick.SystemCommand.WriteFileAsync(data, "../prjs/DOT_PRINT3R/image.rtf");

            Debug.WriteLine("I: File sent successfully");
        }
    }
}
