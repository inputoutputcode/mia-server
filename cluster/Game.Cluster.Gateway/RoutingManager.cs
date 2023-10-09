using System;
using System.Collections.Generic;
using System.Text;


namespace Gateway
{
    public class RoutingManager
    {
        public void ReceiveCommand(byte[] receivedBytes)
        {
            string receivedCommand = Encoding.UTF8.GetString(receivedBytes, 0, receivedBytes.Length);
        }
    }
}
