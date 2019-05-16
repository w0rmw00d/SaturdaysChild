using System;
using System.Collections.Generic;

namespace SaturdaysChild.Models.ViewModels
{
    public class BugViewModel
    {
        public List<BugItems> OpenBugList { get; set; }
    }

    public class BugItems
    {
        public int Id { get; set; }
        public DateTime BugTime { get; set; }
        public string BugType { get; set; }
        public string BugDescription { get; set; }
    }

    public class BugHistoryViewModel
    {
        public List<BugItems> ClosedBugList { get; set; }
    }

    public class BugSearchViewModel
    {
        public DateTime BugStart { get; set; }
        public DateTime BugEnd { get; set; }
        public string BugType { get; set; }
    }
}