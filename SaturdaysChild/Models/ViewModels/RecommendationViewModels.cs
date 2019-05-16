using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// NOTE: Variables prefaced by Recc refer to the book/paper being
/// discussed. All other variables refer to the recommendation itself.
/// All Author values are set based on current session values.
/// </summary>
namespace SaturdaysChild.Models.ViewModels
{
    public class AddReccViewModel
    {
        public string Author { get; set; }

        [Required]
        [Display(Name ="Author of Book/Paper")]
        [StringLength(50, ErrorMessage = "The name cannot exceed 50 characters.")]
        public string ReccAuthor { get; set; }

        [Required]
        [Display(Name = "Title of Book/Paper")]
        [StringLength(150, ErrorMessage = "The title cannot exceed 150 characters.")]
        public string ReccTitle { get; set; }

        [Required]
        [Display(Name = "Link to Book/Paper")]
        public string ReccLink { get; set; }

        [Required]
        [Display(Name = "Title")]
        [StringLength(150, ErrorMessage = "The title cannot exceed 150 characters.")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Entry")]
        [DataType(DataType.MultilineText)]
        public string Entry { get; set; }

        [Required]
        [Display(Name = "Tags")]
        [StringLength(75, ErrorMessage = "The tags cannot exceed 75 characters.")]
        public string Tags { get; set; }
    }

    public class DetailReccViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Date Published")]
        public DateTime EntryDate { get; set; }

        [Display(Name = "Author of Book/Paper")]
        public string ReccAuthor { get; set; }

        [Display(Name = "Title of Book/Paper")]
        public string ReccTitle { get; set; }

        public string ReccLink { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Entry")]
        public string Entry { get; set; }

        [Display(Name = "Tags")]
        public string Tags { get; set; }
    }

    public class EditReccViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Author")]
        [StringLength(50, ErrorMessage = "The name cannot exceed 50 characters.")]
        public string Author { get; set; }

        [Required]
        [Display(Name = "Author of Recommendation Discussed")]
        [StringLength(50, ErrorMessage = "The name cannot exceed 50 characters.")]
        public string ReccAuthor { get; set; }

        [Required]
        [Display(Name = "Title of Recommendation Discussed")]
        [StringLength(150, ErrorMessage = "The title cannot exceed 150 characters.")]
        public string ReccTitle { get; set; }

        [Required]
        [Display(Name = "Link to Recommendation Discussed")]
        public string ReccLink { get; set; }

        [Required]
        [Display(Name = "Title")]
        [StringLength(150, ErrorMessage = "The title cannot exceed 150 characters.")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Entry")]
        [DataType(DataType.MultilineText)]
        public string Entry { get; set; }

        [Required]
        [Display(Name = "Tags")]
        [StringLength(75, ErrorMessage = "The tags cannot exceed 75 characters.")]
        public string Tags { get; set; }
    }

    // used to display recommendations to anon visitors to the site
    public class ReccDisplayViewModel
    {
        public DateTime End { get; set; }

        public List<DetailReccViewModel> ReccList { get; set; }
    }
}