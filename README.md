# EventHubDemo
Showcases how to use Azure event hubs and Azure Event Grid to build and deploy code

## Usage

The application can send messages to either Azure EventHub or Azure Event Grid based on a command line flag:

- **EventHub (default)**: `dotnet run` or `dotnet run eventhub`
- **Event Grid**: `dotnet run eventgrid`

## Configuration

Before running, update the connection strings in Program.cs:

- For EventHub: `EventHubConnectionString` and `EventHubName`
- For Event Grid: `EventGridEndpoint` and `EventGridAccessKey`

The application reads custom events from `FeedData.csv` and sends them to the selected service.
