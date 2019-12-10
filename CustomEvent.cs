using System;

namespace EventHubMsgs
{
    public class CustomEvent
    {
        public string ID {get; set;}
        public string DeviceID {get; set;}
        public double Temp {get; set;}
        public double Humidity {get; set;}
        public DateTime CaptureTime {get; set;}
    }
}
