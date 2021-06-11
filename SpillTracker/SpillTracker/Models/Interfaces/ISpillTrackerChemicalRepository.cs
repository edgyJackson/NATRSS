using SpillTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpillTracker.Models.Interfaces
{
    public interface ISpillTrackerChemicalRepository : IRepository<Chemical>
    {
        bool ChemExists(int id);

        Chemical GetChemByCAS(string casNumber);

        List<Chemical> ByFirstLetter(string l);
        Task<List<Chemical>> getChemicalByLetterOrderByNameAsync(string l);
        Task<List<Chemical>> getHashTagAsync();
        Task<List<Chemical>> OrderByNameAsync();
        List<Chemical> getChemicalByLetterOrderByName(string l);
        List<Chemical> getHashTag();
        List<Chemical> OrderByName();
        Task<bool> TheCIDIsNullAsync(string cas);
        Task<bool> TheMolecularWeightIsNullAsync(string cas);
        Task<bool> TheDensityIsNullAsync(string cas);
        Task<bool> TheVaporPressureIsNullAsync(string cas);

    }
}