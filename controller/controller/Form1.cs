using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using SharpOSC;

namespace controller
{
    public partial class Form1 : Form
    {

        private readonly int receivePort = 13574;
        private readonly int senderPort = 13575;

        private UDPListener listener;
        private Thread listenerThread;

        private UDPSender sender;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This function is called when the form is loaded. It gets the local IP address of the computer.
        /// </summary>
        /// <param name="sender">This is for the object overload.</param>
        /// <param name="e">This is the event.</param>
        private void Controller_Load(object sender, EventArgs e)
        {
            localIP.Text = string.Join("\n", GetLocalIPAdress().ToArray());
        }

        /// <summary>
        /// This function gets the local IP address of the computer.
        /// </summary>
        /// <returns>A list of ip adresses.</returns>
        private List<string> GetLocalIPAdress()
        {
            List<string> localIPs = new List<string>();
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIPs.Add(ip.ToString());
                    break;
                }
            }
            return localIPs;
        }

        /// <summary>
        /// This function is called when the start button is clicked. It starts the listener thread.
        /// </summary>
        /// <param name="sender">This is for the object overload.</param>
        /// <param name="e">This is the event.</param>
        private void Start_Click(object sender, EventArgs e)
        {
            listenerThread = new Thread(ListenToSender);
            listenerThread.IsBackground = true;
            listenerThread.Start();

            Console.WriteLine("OSC listener started.");
            Console.WriteLine("Listening on port: {0}", receivePort);
            Console.WriteLine("Sending to IP: {0}", receiverIP.Text + ":" + senderPort + "\n");
        }

        /// <summary>
        /// This function is called when the cancel button is clicked. It stops the listener and closes the connection.
        /// </summary>
        /// <param name="sender">This is for the object overload.</param>
        /// <param name="e">This is the event.</param>
        private void Cancel_Click(object sender, EventArgs e)
        {
            if (listenerThread != null && listenerThread.IsAlive)
            {
                listenerThread.Abort();
                listener.Close();
                Debug.Print("OSC listener stopped.");
            }
            else
            {
                Debug.Print(listenerThread == null ? "No listener thread to stop." : "Listener thread is not alive.");
            }
        }

        /// <summary>
        /// This function listens to the sender and waits for a message. When a message is received, it is converted and sent to the receiver.
        /// </summary>
        private void ListenToSender()
        {
            listener = new UDPListener(receivePort);

            while (true)
            {
                var packet = listener.Receive();
                if (packet is OscMessage message)
                {
                    Console.WriteLine("Received a message for address: " + message.Address);
                    foreach (var arg in message.Arguments)
                    {
                        if (arg is List<System.Object>)
                        {
                            Console.WriteLine("Found a list of strings!");

                            foreach (var item in arg as List<System.Object>)
                            {
                                Console.WriteLine("Message argument: " + item);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Message argument: " + arg);
                            Console.WriteLine("Message argument type: " + arg.GetType());
                        }
                    }

                    if (receiverIP.Text != "")
                    {
                        SendToReceiver(ConvertMessage());
                    }
                }
            }
        }

        /// <summary>
        /// This function sends a message to the receiver, in this case the output system.
        /// </summary>
        /// <param name="message">The message is a simple osc message.</param>
        private void SendToReceiver(OscMessage message)
        {
            sender = new SharpOSC.UDPSender(receiverIP.Text, senderPort);

            Console.WriteLine("Sending message to: " + receiverIP.Text + ":" + senderPort);
            sender.Send(message);
        }

        /// <summary>
        /// This function is called when a message is received. The logic is still missing.
        /// </summary>
        /// <param name="message">This is the received message from OSC.</param>
        /// <returns>This function returns a converted message for the output system.</returns>
        private OscMessage ConvertMessage()
        {
            // Implement conversion logic here
            Console.WriteLine("Converting message for output.");

            OscMessage message = new OscMessage("/output");

            message.Arguments.Add("Converted for output");

            // I don't know how to send arrays with SharpOSC yet
            // This might be helpful https://github.com/ValdemarOrn/SharpOSC/blob/master/SharpOSC/OscMessage.cs
            message.Arguments.Add(9);
            message.Arguments.Add(7);
            message.Arguments.Add(5);

            return message;
        }
    }
}
