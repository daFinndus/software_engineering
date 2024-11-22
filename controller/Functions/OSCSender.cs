using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharpOSC;

namespace controller_crossplatform.Functions
{
    /// <summary>
    /// This is the class that sends the OSC messages to the receiver.
    /// </summary>
    /// <param name="ip">Local IP Address of the receiver.</param>
    /// <param name="port">Port of the receiver.</param>
    internal class OSCSender(string ip, int port)
    {
        protected SharpOSC.UDPSender? sender;

        public string receiverIP = ip;
        public int receiverPort = port;

        /// <summary>
        /// This function sends a message to the receiver, in this case the output system.
        /// </summary>
        /// <param name="message">The message is a simple osc message.</param>
        public void SendToReceiver(OscMessage message)
        {
            Console.WriteLine("\nGoing to send the following message!");
            message = OSCFunctions.ConvertMessage(message);
            OSCFunctions.LogMessage(message);

            sender = new SharpOSC.UDPSender(receiverIP, receiverPort);
            sender.Send(message);
        }
    }
}
