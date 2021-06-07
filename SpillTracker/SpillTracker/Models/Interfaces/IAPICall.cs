using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpillTracker.Models.Interfaces
{
    public interface IAPICall
    {
        string DoAPICall(string url);
    }
}
