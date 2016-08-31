using System;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DockerCoins.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting DockerCoins Worker");
            
            string redisHostname = "redis"; // TODO: get from args or environment
            Network.LogAllIPsForHostName(redisHostname);
            string redisIp = Network.GetIPFromHostName(redisHostname);
            if (redisIp == null)
            {
                Console.WriteLine("Failed to lookup IP address for Redis. Exiting.......");
                return;            
            }
            
            while (true)
            {
                try
                {
                    Miner miner = new Miner(1, redisIp, "http://hasher:8001", "http://rng:8002/32");
                    miner.DoWork();   
                }
                catch (System.Exception err)
                {
                    
                    Console.WriteLine("Error occured.\n" + err.ToString());
                    Console.WriteLine("Waiting 10s and restarting");
                    Thread.Sleep(10000);
                }
            }
            
        }
    }
}
