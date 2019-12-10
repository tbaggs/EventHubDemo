using System;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

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

            await SendMessagesToEventHub(1000);

            await eventHubClient.CloseAsync();

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        // Uses the event hub client to send 100 messages to the event hub.
        private static async Task SendMessagesToEventHub(int numMessagesToSend)
        {
            CustomEvent msg = new CustomEvent();

            for (var i = 0; i < numMessagesToSend; i++)
            {
                try
                {
                    msg.ID = Guid.NewGuid().ToString();
                    msg.DeviceID = "TestClient";
                    msg.CaptureTime = DateTime.UtcNow;
                    msg.Temp = NextRandomRange(22.0, 26.0);
                    msg.Humidity = NextRandomRange(66.0, 68.0);

                    string msgJson = JsonSerializer.Serialize(msg);  

                    Console.WriteLine($"Sending message: {msgJson}");
                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(msgJson)));

                    await Task.Delay(1000);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(10);
            }

            Console.WriteLine($"{numMessagesToSend} messages sent.");
        }

        private static double NextRandomRange(double minimum, double maximum)
        {
            Random rand = new Random();
            return rand.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
