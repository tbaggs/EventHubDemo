using System;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using CsvHelper;
using Azure.Messaging.EventGrid;

namespace EventHubMsgs
{
    class Program
    {
        private static EventHubClient eventHubClient;
        private static EventGridPublisherClient eventGridClient;
        private const string EventHubConnectionString = "";
        private const string EventHubName = "";
        private const string EventGridEndpoint = "";
        private const string EventGridAccessKey = "";
        
        // Flag to determine which service to use: "eventhub" or "eventgrid"
        private static string serviceType = "eventhub";
        
        public static void Main(string[] args)
         {
             // Parse command line arguments to determine service type
             if (args.Length > 0 && (args[0].ToLower() == "eventhub" || args[0].ToLower() == "eventgrid"))
             {
                 serviceType = args[0].ToLower();
             }
             
             Console.WriteLine($"Using service type: {serviceType}");
             MainAsync(args).GetAwaiter().GetResult();
         }


        private static async Task MainAsync(string[] args)
        {
            // Initialize the appropriate client based on service type
            if (serviceType == "eventhub")
            {
                var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
                {
                    EntityPath = EventHubName
                };
                eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            }
            else if (serviceType == "eventgrid")
            {
                eventGridClient = new EventGridPublisherClient(new Uri(EventGridEndpoint), new Azure.AzureKeyCredential(EventGridAccessKey));
            }

            using (var reader = new StreamReader("FeedData.csv"))
            using (var csv = new CsvReader(reader))
            {    
                var records = csv.GetRecords<CustomEvent>();

                foreach (var rec in records)
                {
                    await SendMessage(rec);
                    await Task.Delay(1000);
                }
            }

            // Close the appropriate client
            if (serviceType == "eventhub" && eventHubClient != null)
            {
                await eventHubClient.CloseAsync();
            }

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        // Uses the appropriate client to send messages to EventHub or Event Grid
        private static async Task SendMessage(CustomEvent customEvent)
        {
            try
            {
                string msgJson = JsonSerializer.Serialize(customEvent);  

                Console.WriteLine($"Sending message to {serviceType}: {msgJson}");
                
                if (serviceType == "eventhub")
                {
                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(msgJson)));
                }
                else if (serviceType == "eventgrid")
                {
                    var eventGridEvent = new EventGridEvent(
                        subject: "CustomEvent",
                        eventType: "EventHubMsgs.CustomEvent",
                        dataVersion: "1.0",
                        data: customEvent
                    );
                    
                    await eventGridClient.SendEventAsync(eventGridEvent);
                }
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
