using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SaturdaysChild.Models.ViewModels
{
    // NOTE: Add in SupportController may be called either from SupportController or from 
    // BugController. If called from BugController, will have BugId, otherwise will not.
    public class AddSupportTicket
    {
        [Required]
        [Display(Name = "Client")]
        public string Client { get; set; }

        [Required]
        [Display(Name = "Contract")]
        public string Contract { get; set; }

        [Required]
        [Display(Name = "Contact Name")]
        [StringLength(75, ErrorMessage = "The contact name cannot be longer than 75 characters.")]
        public string ContactName { get; set; }

        [Required]
        [Display(Name = "Contact Phone/Email")]
        [StringLength(50, ErrorMessage = "The contact method cannot be longer than 50 characters.")]
        public string Contact { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public int? BugId { get; set; }
    }
    
    public class DisplaySupportTicket
    {
        public int Id { get; set; }

        [Display(Name = "Employee")]
        public string Employee { get; set; }

        [Display(Name = "Client")]
        public string Client { get; set; }

        [Display(Name = "Contract")]
        public string Contract { get; set; }

        [Display(Name = "Ticket Open")]
        public DateTime TicketStart { get; set; }

        [Display(Name = "Ticket Closed")]
        public DateTime? TicketEnd { get; set; }
    }

    public class EditSupportTicket
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Employee")]
        [StringLength(50, ErrorMessage = "The name cannot be longer than 50 characters.")]
        public string Employee { get; set; }

        [Required]
        [Display(Name = "Client")]
        [StringLength(50, ErrorMessage = "The client cannot exceed 50 characters.")]
        public string Client { get; set; }

        [Required]
        [Display(Name = "Contract")]
        public string Contract { get; set; }

        [Required]
        [Display(Name = "Contact Name")]
        [StringLength(75, ErrorMessage = "The contact name cannot be longer than 75 characters.")]
        public string ContactName { get; set; }

        [Required]
        [Display(Name = "Contact Phone/Email")]
        [StringLength(50, ErrorMessage = "The contact method cannot be longer than 50 characters.")]
        public string Contact { get; set; }

        [Required]
        [Display(Name = "Ticket Open")]
        public DateTime TicketStart { get; set; }

        [Display(Name = "Ticket Closed")]
        public DateTime? TicketEnd { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public int BugId { get; set; }
    }

    // displays support tickets for admin users
    public class SupportDisplay
    {
        public List<SupportItems> SupportList { get; set; }
        public class SupportItems
        {
            public int Id { get; set; }
            public string Employee { get; set; }
            public string Client { get; set; }
            public DateTime TicketStart { get; set; }
            public DateTime? TicketEnd { get; set; }
            public string Notes { get; set; }
        }
    }
}