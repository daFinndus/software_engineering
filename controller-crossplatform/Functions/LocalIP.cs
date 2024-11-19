using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace controller_crossplatform.Functions
{
    internal class LocalIP
    {
        /// <summary>
        /// This function gets the local IP address of the computer.
        /// </summary>
        /// <returns>A list of ip adresses.</returns>
        public static List<string> GetLocalIPAddress()
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
    }
}
