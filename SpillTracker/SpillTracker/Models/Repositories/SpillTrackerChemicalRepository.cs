using SpillTracker.Models;
using SpillTracker.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace SpillTracker.Models.Repositories
{
    public class SpillTrackerChemicalRepository : Repository<Chemical>, ISpillTrackerChemicalRepository 
    {
        public SpillTrackerChemicalRepository(SpillTrackerDbContext ctx) : base(ctx)
        {

        }
        public virtual bool ChemExists(int id)
        {
            return _dbSet.Any(f => f.Id == id);
        }

        public virtual Chemical GetChemByCAS(string casNumber)
        {
            return _dbSet.Where(c => c.CasNum == casNumber).FirstOrDefault();
        }

        public virtual async Task<List<Chemical>> OrderByNameAsync()
        {
            return await _context.Chemicals.OrderBy(x => x.Name).ToListAsync();
        }

        public virtual async Task<List<Chemical>> getHashTagAsync()
        {
            var list = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z".Split(" ").ToList();
            return await _context.Chemicals.Where(c => !list.Contains(c.Name.Substring(0, 1))).OrderBy(x => x.Name).ToListAsync();
        }
        public virtual async Task<List<Chemical>> getChemicalByLetterOrderByNameAsync(string l)
        {
            return await _context.Chemicals.Where(c => c.Name.Substring(0, l.Length).Contains(l)).OrderBy(x => x.Name).ToListAsync();
        }
        public virtual List<Chemical> ByFirstLetter(string l) 
        {
            //var list = new List<string> "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            var list = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z".Split(" ");
            var all = _dbSet.OrderBy(x=>x.Name);
            var letter = new List<Chemical>();
            var hashtag = new List<Chemical>();
            letter = _dbSet.Where(c => c.Name.Substring(0,1).Contains(l)).OrderBy(x => x.Name).ToList();
            hashtag = _dbSet.Where(c => !list.Contains(c.Name.Substring(0,1))).OrderBy(x => x.Name).ToList();
            //_logger.LogInformation(sort.letterInput);(x => x.Name).ToList();
            
            if(l == null) 
            {
                return _dbSet.OrderBy(x=>x.Name).ToList();
            }
            else if(l.Length > 1)
            {
                letter = _dbSet.Where(c => c.Name.Substring(0,l.Length).Contains(l)).OrderBy(x => x.Name).ToList();
                return letter;
            }
            else if(l != "#") 
            {
                return letter;
            }
            else if (l == "#")
            {
                return hashtag;
            }
            else
            {
               return _dbSet.OrderBy(x=>x.Name).ToList();
            }   
        }

        public virtual async Task<bool> TheCIDIsNullAsync(string cas)
        {
            return await _context.Chemicals.Where(a => a.CasNum == cas).Select(x => x.PubChemCid).FirstOrDefaultAsync() == null;
        }
        public virtual async Task<bool> TheMolecularWeightIsNullAsync(string cas)
        {
            return await _context.Chemicals.Where(a => a.CasNum == cas).Select(x => x.MolecularWeight).FirstOrDefaultAsync() == null;
        }
        public virtual async Task<bool> TheDensityIsNullAsync(string cas)
        {
            return await _context.Chemicals.Where(a => a.CasNum == cas).Select(x => x.Density).FirstOrDefaultAsync() == null;
        }
        public virtual async Task<bool> TheVaporPressureIsNullAsync(string cas)
        {
            return await _context.Chemicals.Where(a => a.CasNum == cas).Select(x => x.VaporPressure).FirstOrDefaultAsync() == null;
        }
    }
}