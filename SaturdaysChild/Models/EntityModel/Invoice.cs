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
    
    public partial class Invoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Invoice()
        {
            this.Timesheets = new HashSet<Timesheet>();
        }
    
        public int Id { get; set; }
        public string InternalId { get; set; }
        public int ClientId { get; set; }
        public int ContractId { get; set; }
        public System.DateTime DateIssued { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public decimal Amount { get; set; }
    
        public virtual Client Client { get; set; }
        public virtual Contract Contract { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Timesheet> Timesheets { get; set; }
    }
}
