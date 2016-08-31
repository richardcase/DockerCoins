using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace DockerCoins.Worker
{
    public class Miner
    {
        public int IntervalInSeconds { get; private set; }

        private readonly string redisUrl;
        private readonly string hasherUrl;
        private readonly string rngUrl;

        private ConnectionMultiplexer redis;

        public Miner(int interval, string redisUrl, string hasherUrl, string rngUrl) 
        {
            this.IntervalInSeconds = interval;
            this.redisUrl = redisUrl;
            this.hasherUrl = hasherUrl;
            this.rngUrl = rngUrl;
        }

        public void DoWork()
        {
            ConfigurationOptions config = new ConfigurationOptions
            {
                EndPoints = 
                {
                    { this.redisUrl, 6379 }
                },
                AbortOnConnectFail=false
            };



            redis = ConnectionMultiplexer.Connect(config);
            
            DateTime deadLine = DateTime.MinValue;
            int loopsDone = 0;

            while (true) {
                if (DateTime.Now > deadLine) 
                {
                    Console.WriteLine("{0} unit of work done, updating hash counter", loopsDone);
                    IDatabase db = redis.GetDatabase();
                    db.StringIncrement("hashes",loopsDone);

                    loopsDone = 0;
                    deadLine = DateTime.Now.AddSeconds(this.IntervalInSeconds);
                }
                WorkOnce();
                loopsDone += 1;
            }  
        }

        private void WorkOnce()
        {
            Console.WriteLine("Doing one unit of work");
            Thread.Sleep(100);

            Task<string> randomTask = GetRandomBytesAsync();
            randomTask.Wait();
            string randomBytes = randomTask.Result;
            //Console.WriteLine("Returned from RNG: {0}", randomBytes);

            Task<string> hashTask = HashBytesAsync(randomBytes);
            hashTask.Wait();
            string hexHash = hashTask.Result; 
            //Console.WriteLine("Returned from Hasher: {0}", hexHash);

            if (hexHash.StartsWith("0") == false) 
            {
                Console.WriteLine("No coin found");
                return;
            }
            Console.WriteLine("Coin found: {0}", hexHash);
            IDatabase db = redis.GetDatabase();
            db.HashSet("wallet", randomBytes, hexHash );
        }

        private async Task<string> GetRandomBytesAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    request.RequestUri = new Uri (this.rngUrl);
                    request.Method = HttpMethod.Get;
                    request.Headers.Accept.Clear();

                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        return responseBody;
                    }
                }

            }
            return "";
        }

        private async Task<string> HashBytesAsync(string bytesToHash)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    request.RequestUri = new Uri (this.hasherUrl);
                    request.Method = HttpMethod.Post;
                    request.Headers.Accept.Clear();
                   
                    using(StringContent content = new StringContent(bytesToHash))
                    {
                        request.Content = content;

                        var response = await client.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseBody = await response.Content.ReadAsStringAsync();
                            return responseBody;
                        }
                    }
                }

            }
            return "";
        }
    }
}