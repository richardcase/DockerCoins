using System;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DockerCoins.Worker
{
    public static class Network
    {
        public static string GetIPFromHostName(string hostName)
        {
            var task = Dns.GetHostEntryAsync(hostName);
            task.Wait();
            IPHostEntry result = task.Result;
            
            Console.WriteLine("Found {0} IP addresses for host {1}", result.AddressList.Length, hostName);
            foreach (IPAddress address in result.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork) 
                {
                    return address.ToString();
                }
            }

            return null;
        }
    }
}