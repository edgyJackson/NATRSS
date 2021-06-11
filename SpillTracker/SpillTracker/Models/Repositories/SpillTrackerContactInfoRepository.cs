using SpillTracker.Models;
using SpillTracker.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpillTracker.Models.Repositories
{
    public class SpillTrackerContactInfoRepository : Repository<ContactInfo>, ISpillTrackerContactInfoRepository
    {
        public SpillTrackerContactInfoRepository(SpillTrackerDbContext ctx) : base(ctx)
        {

        }

        public virtual bool ContactInfoExists(ContactInfo ci)
        {
            return _dbSet.Any(x => x.AgencyName == ci.AgencyName && x.PhoneNumber == ci.PhoneNumber && x.State == ci.State);
        }

        public virtual ContactInfo GetContactInfoByAgencyName(string name)
        {
            return _dbSet.Where(x => x.AgencyName == name).FirstOrDefault();
        }

        public virtual List<ContactInfo> ByState(string state)
        {
            return _dbSet.OrderBy(x => x.State).ToList();
        }

    }
}