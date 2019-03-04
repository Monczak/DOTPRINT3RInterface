using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    static class FileSender
    {
        public static void ConnectAndSend()
        {
            EV3.ConnectToBrick();
            if (EV3.Connected)
            {
                EV3.TestConnection();
            }
            EV3.Disconnect();
            
        }
    }
}
