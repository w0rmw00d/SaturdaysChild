//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SaturdaysChild.Models.EntityModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Blog
    {
        public int Id { get; set; }
        public System.DateTime EntryDate { get; set; }
        public int AuthorId { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Entry { get; set; }
        public string Tags { get; set; }
    
        public virtual Employee Employee { get; set; }
    }
}
