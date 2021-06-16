using SpillTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpillTracker.Models.Interfaces
{
    public interface ISpillTrackerContactInfoRepository : IRepository<ContactInfo>
    {
        bool ContactInfoExists(ContactInfo ci);

        ContactInfo GetContactInfoByAgencyName(string AgencyName);

        List<ContactInfo> ByState(string state);
    }
}