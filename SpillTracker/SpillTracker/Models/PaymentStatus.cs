using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpillTracker.Models
{
    public enum PaymentStatus
    {
        Pending = 10,

        Authorized = 20,

        Paid = 30,

        Cancelled = 50,
    }
}
