using System;
using System.Collections.Generic;

namespace SaturdaysChild.Models.ViewModels
{
    // displays people currently logged in
    public class AccessDisplay
    {
        public List<AccessItems> AccessList { get; set; }
        public class AccessItems
        {
            public int Id { get; set; }
            public string Pseudonym { get; set; }
            public DateTime Login { get; set; }
            public DateTime? Logout { get; set; }
            public string IP { get; set; }
            public string MAC { get; set; }
        }
    }
}