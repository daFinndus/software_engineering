using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SharpOSC;

namespace controller_crossplatform.Functions
{
    /// <summary>
    /// This class is responsible for receiving OSC messages.
    /// </summary>
    /// <param name="sender">The sender that manages the message sending.</param>
    /// <param name="port">The port to listen to.</param>
    internal class OSCReceiver(OSCSender sender, int port)
    {
        private UDPListener? listener;

        protected int receiverPort = port;

        /// <summary>
        /// This function listens to the sender and waits for a message. When a message is received, it is converted and sent to the receiver.
        /// </summary>
        public void ListenToSender()
        {
            listener = new UDPListener(receiverPort);

            while (true)
            {
                var packet = listener.Receive();
                if (packet is OscMessage message)
                {
                    Console.WriteLine("\nReceived a message for address: " + message.Address);

                    if (message.Address == "/connect")
                    {
                        Console.WriteLine("Successfully connected to the smartphone!");
                    }
                    else
                    {
                        OSCFunctions.LogMessage(message);
                    }

                    sender.SendToReceiver(message);
                }
            }
        }
    }
}