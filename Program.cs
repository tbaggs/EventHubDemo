using System;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using CsvHelper;

namespace EventHubMsgs
{
    class Program
    {
        private static EventHubClient eventHubClient;
        private const string EventHubConnectionString = "";
        private const string EventHubName = "";
        
        public static void Main(string[] args)
         {
             MainAsync(args).GetAwaiter().GetResult();
         }


        private static async Task MainAsync(string[] args)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            using (var reader = new StreamReader("FeedData.csv"))
            using (var csv = new CsvReader(reader))
            {    
                var records = csv.GetRecords<CustomEvent>();

                foreach (var rec in records)
                {
                    await SendMessagesToEventHub(rec);
                    await Task.Delay(1000);
                }
            }

            await eventHubClient.CloseAsync();

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        // Uses the event hub client to send 100 messages to the event hub.
        private static async Task SendMessagesToEventHub(CustomEvent customEvent)
        {
            try
            {
                string msgJson = JsonSerializer.Serialize(customEvent);  

                Console.WriteLine($"Sending message: {msgJson}");
                await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(msgJson)));
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
            }
        }

        private static double NextRandomRange(double minimum, double maximum)
        {
            Random rand = new Random();
            return rand.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
