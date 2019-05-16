using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// NOTE: All Author values are set based on current session values.
/// </summary>
namespace SaturdaysChild.Models.ViewModels
{
    public class AddBlogViewModel
    {
        [Required]
        public string Author { get; set; }

        [Required]
        [Display(Name = "Tags")]
        [StringLength(75, ErrorMessage = "The tags cannot exceed 75 characters.")]
        public string Tags { get; set; }

        [Required]
        [Display(Name = "Title")]
        [StringLength(150, ErrorMessage = "The title cannot exceed 150 characters.")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Entry")]
        [DataType(DataType.MultilineText)]
        public string Entry { get; set; }
    }

    // used to display blogs to anon visitors to the site
    public class BlogDisplayViewModel
    {
        public IEnumerable<DetailBlogViewModel> BlogList { get; set; }
    }

    public class DetailBlogViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Date Published")]
        public DateTime EntryDate { get; set; }

        [Display(Name = "Author")]
        public string Author { get; set; }

        [Display(Name = "Tags")]
        public string Tags { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Entry")]
        [DataType(DataType.MultilineText)]
        public string Entry { get; set; }
    }
    
    public class EditBlogViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Date Published")]
        public DateTime EntryDate { get; set; }

        [Required]
        [Display(Name = "Author")]
        [StringLength(50, ErrorMessage = "The name cannot exceed 50 characters.")]
        public string Author { get; set; }

        [Required]
        [Display(Name = "Tags")]
        [StringLength(75, ErrorMessage = "The tags cannot exceed 75 characters.")]
        public string Tags { get; set; }

        [Required]
        [Display(Name = "Title")]
        [StringLength(150, ErrorMessage = "The title cannot exceed 150 characters.")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Entry")]
        [DataType(DataType.MultilineText)]
        public string Entry { get; set; }
    }
}