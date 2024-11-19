using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpOSC;

namespace controller_crossplatform.Functions
{
    internal class OSCFunctions
    {
        public static OscMessage ConvertMessage(OscMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            else
            {
                // Implement conversion logic here
                Console.WriteLine("Converting message for output.");

                message = new OscMessage("/output");

                message.Arguments.Add("Converted for output");

                // I don't know how to send arrays with SharpOSC yet
                // This might be helpful https://github.com/ValdemarOrn/SharpOSC/blob/master/SharpOSC/OscMessage.cs
                message.Arguments.Add(9);
                message.Arguments.Add(7);
                message.Arguments.Add(5);
            }

            return message;
        }

        public static void LogMessage(OscMessage message)
        {
            foreach (var arg in message.Arguments)
            {
                if (arg is List<System.Object> list)
                {
                    Console.WriteLine("Found a list of strings!");

                    foreach (var item in list)
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
        }
    }
}
