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

            string redisHostName = Environment.GetEnvironmentVariable("DOCKERCOINS_REDIS");
            if (string.IsNullOrEmpty(redisHostName))
            {
                Console.WriteLine("ERROR: You must set the DOCKERCOINS_REDIS environment variable");
                return;
            }
            string hasherUrl = Environment.GetEnvironmentVariable("DOCKERCOINS_HASHER");
            if (string.IsNullOrEmpty(hasherUrl))
            {
                Console.WriteLine("ERROR: You must set the DOCKERCOINS_HASHER environment variable");
                return;
            }
            string rngUrl = Environment.GetEnvironmentVariable("DOCKERCOINS_RNG");
            if (string.IsNullOrEmpty(rngUrl))
            {
                Console.WriteLine("ERROR: You must set the DOCKERCOINS_RNG environment variable");
                return;
            }

            Network.LogAllIPsForHostName(redisHostName);
            string redisIp = Network.GetIPFromHostName(redisHostName);
            if (redisIp == null)
            {
                Console.WriteLine("ERROR: Failed to lookup IP address for Redis. Exiting.......");
                return;            
            }
            
            while (true)
            {
                try
                {
                    Miner miner = new Miner(1, redisIp, hasherUrl, rngUrl);
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
