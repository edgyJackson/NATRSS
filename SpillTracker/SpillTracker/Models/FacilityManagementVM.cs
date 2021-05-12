﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpillTracker.Models
{
    public class FacilityManagementVM
    {
        public IEnumerable<Facility> Facility { get; set; }

        public IEnumerable<FacilityChemical> FacilityChemicals { get; set; }

        public Stuser user {get; set;}

        public IEnumerable<Company> companies {get; set;}


        public string codes {get; set;}
    }
}
