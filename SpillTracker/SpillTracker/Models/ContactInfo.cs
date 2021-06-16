using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace SpillTracker.Models
{
    [Table("ContactInfo")]
    public partial class ContactInfo
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string AgencyName { get; set; }
        [StringLength(20)]
        [Required]
        public string PhoneNumber { get; set; }
        [StringLength(35)]
        [Required]
        public string State { get; set; }
    }
}
