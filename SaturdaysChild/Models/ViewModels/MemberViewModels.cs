using System;
using System.Collections.Generic;

namespace SaturdaysChild.Models.ViewModels
{
    // view model for first page seen after log in to system. displays messages and alerts for person logged in.
    public class MemberIndexViewModel
    {
        public string Name { get; set; }
        public List<AlertData> Alerts { get; set; }
        public List<MessageData> Messages { get; set; }

        public class AlertData
        {
            public DateTime? Date { get; set; }
            public string Alert { get; set; }
        }
        public class MessageData
        {
            public DateTime? Date { get; set; }
            public string Message { get; set; }
            public string From { get; set; }
        }
    }
}