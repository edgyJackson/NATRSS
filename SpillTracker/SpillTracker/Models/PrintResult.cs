using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SpillTracker.Models
{
    public class PrintResult
    {


        [Key]
        [Column("ID")]
        public int Id { get; set; }

        public Form theFormToPrint { get; set; }
        public Stuser theFacilityManager { get; set; }

        public string results { get; set; }
    }
}
