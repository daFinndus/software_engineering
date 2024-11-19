using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharpOSC;

namespace controller_crossplatform.Functions
{
    internal class OSCSender(string receiverIP, int senderPort)
    {
        private SharpOSC.UDPSender? sender;

        /// <summary>
        /// This function sends a message to the receiver, in this case the output system.
        /// </summary>
        /// <param name="message">The message is a simple osc message.</param>
        public void SendToReceiver(OscMessage message)
        {
            Console.WriteLine("\nGoing to send the following message!");
            message = OSCFunctions.ConvertMessage(message);
            OSCFunctions.LogMessage(message);

            sender = new SharpOSC.UDPSender(receiverIP, senderPort);
            sender.Send(message);
        }
    }
}
