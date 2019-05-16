using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Class contains the view models for the default search and results.
/// </summary>
namespace SaturdaysChild.Models.ViewModels
{
    public class ResultsViewModel
    {
        public IEnumerable<ResultData> Results { get; set; }
        public class ResultData
        {
            public int Id { get; set; }
            public DateTime PubDate { get; set; }
            public string Author { get; set; }
            public string Title { get; set; }
            public string Tags { get; set; }
            public string Link { get; set; }
            public string Entry { get; set; }
        }
    }

    public class SearchViewModel
    {
        [Required]
        [Display(Name = "Search")]
        [StringLength(75, ErrorMessage = "The search term cannot exceed 75 characters.")]
        public string SearchString { get; set; }

        [Display(Name = "Author")]
        [StringLength(50, ErrorMessage = "The author cannot exceed 50 characters.")]
        public string Author { get; set; }

        [Display(Name = "Tags")]
        [StringLength(75, ErrorMessage = "The tags cannot exceed 75 characters.")]
        public string Tags { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        public IEnumerable<string> AuthorList { get; set; }
        public IEnumerable<string> TypeList { get; set; }
    }
}