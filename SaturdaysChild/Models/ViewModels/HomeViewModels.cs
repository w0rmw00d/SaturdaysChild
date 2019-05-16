using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SaturdaysChild.Models.ViewModels
{
    public class BugReportViewModel
    {
        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Bug Encountered At")]
        public DateTime BugTime { get; set; }

        [Required]
        [Display(Name = "Type of Bug")]
        public string BugType { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description of Bug")]
        [StringLength(500, ErrorMessage = "The description cannot exceed 500 characters.")]
        public string BugDescription { get; set; }

        public List<BugTypeModel> BugTypes { get; set; }

        public class BugTypeModel
        {
            public int TypeId { get; set; }
            public string TypeName { get; set; }
        }
    }

    public class ContactViewModel
    {
        [Required]
        [Display(Name = "Name")]
        [StringLength(50, ErrorMessage = "The name cannot exceed 50 characters.")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone]
        [Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Best Time To Contact")]
        public DateTime BestContactStart { get; set; }

        [Required]
        [Display(Name = "Message")]
        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessage = "The message cannot exceed 500 characters.")]
        public string Message { get; set; }
    }

    public class ThanksViewModel
    {
        [Required]
        public string Message { get; set; }
    }
}