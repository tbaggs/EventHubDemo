using System;

namespace EventHubMsgs
{
    public class CustomEvent
    {
        public string Order_No {get; set;}
        public string Item_Id {get; set;}
        public string Item_Description {get; set;}
        public string Status {get; set;}
        public string Description {get; set;}
        public DateTime Status_Date {get; set;}
        public DateTime Createts {get; set;}
        public DateTime Modifyts {get; set;}

    }
}
