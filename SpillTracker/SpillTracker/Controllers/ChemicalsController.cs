 using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SpillTracker.Models;
using SpillTracker.Models.Interfaces;
using SpillTracker.Models.Repositories;
using SpillTracker.Utilities;

namespace SpillTracker.Controllers
{
    public class ChemicalsController : Controller
    {
        /*private readonly SpillTrackerDbContext _context;*/
        private readonly IPugAPI _pug;
        private readonly ISpillTrackerChemicalRepository _chemRepo;

        public ChemicalsController(ISpillTrackerChemicalRepository chemRepo, IPugAPI pug)
        {
           /* _context = context;*/
            _pug = pug;
            _chemRepo = chemRepo;
        }

        // GET: Chemicals
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            /*return View(await _context.Chemicals.ToListAsync());*/
            /*return View(await _context.Chemicals.OrderBy(x => x.Name).ToListAsync());*/
            return View(await _chemRepo.OrderByNameAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> ByFirstLetterAsync(string l)
        {

            //var list = new List<string> "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            var list = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z".Split(" ").ToList();
            var all = await _chemRepo.OrderByNameAsync();
            /*var all = _context.Chemicals.OrderBy(x => x.Name).ToList();*/
            var letter = new List<Chemical>();
            var hashtag = new List<Chemical>();

            letter = await _chemRepo.getChemicalByLetterOrderByNameAsync(l);
            hashtag = await _chemRepo.getHashTagAsync();
            /*letter = _context.Chemicals.Where(c => c.Name.Substring(0, 1).Contains(l)).OrderBy(x => x.Name).ToList();
            hashtag = _context.Chemicals.Where(c => !list.Contains(c.Name.Substring(0, 1))).OrderBy(x => x.Name).ToList();*/
            //_logger.LogInformation(sort.letterInput);(x => x.Name).ToList();

            if (l == null)
            {
                return View("Index", all);
            }
            else if (l.Length > 1)
            {
                letter = await _chemRepo.getChemicalByLetterOrderByNameAsync(l);
                /*letter = _context.Chemicals.Where(c => c.Name.Substring(0, l.Length).Contains(l)).OrderBy(x => x.Name).ToList();*/
                return View("Index", letter);
            }
            else if (l != "#")
            {
                return View("Index", letter);
            }
            else if (l == "#")
            {
                return View("Index", hashtag); //needs adjustment
            }
            else
            {
                return View("Index", all);
            }
        }

        [AllowAnonymous]
        // GET: Chemicals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var chemical = await _chemRepo.FindByIdAsync((int)id);
            /*var chemical = await _context.Chemicals
                .FirstOrDefaultAsync(m => m.Id == id);*/
            if (chemical == null)
            {
                return NotFound();
            }

            ExtraChemData extraData = await GetChemicalPropertiesFromPUGAPIAsync(chemical.CasNum);

            return View(chemical);
        }


        // GET: Chemicals/Create
        // public IActionResult Create()
        // {
        //     return View();
        // }

        // // POST: Chemicals/Create
        // // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([Bind("Id,Name,CasNum,PubChemCid,ReportableQuantity,ReportableQuantityUnits,Density,DensityUnits,MolecularWeight,MolecularWeightUnits,VaporPressure,VaporPressureUnits,CerclaChem,EpcraChem")] Chemical chemical)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         _context.Add(chemical);
        //         await _context.SaveChangesAsync();
        //         return RedirectToAction(nameof(Index));
        //     }
        //     return View(chemical);
        // }

        //GET: Chemicals/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemical = await _chemRepo.FindByIdAsync((int)id);
            /*var chemical = await _context.Chemicals.FindAsync(id);*/
            if (chemical == null)
            {
                return NotFound();
            }
            return View(chemical);
        }

        //  POST: Chemicals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CasNum,PubChemCid,ReportableQuantity,ReportableQuantityUnits,Density,DensityUnits,MolecularWeight,MolecularWeightUnits,VaporPressure,VaporPressureUnits,CerclaChem,EpcraChem")] Chemical chemical)
        {
            if (id != chemical.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {


                    await _chemRepo.AddOrUpdateAsync(chemical);
                    /*_context.Update(chemical);
                    await _context.SaveChangesAsync();*/
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChemicalExists(chemical.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(chemical);
        }

        // GET: Chemicals/Delete/5
        // public async Task<IActionResult> Delete(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }

        //     var chemical = await _context.Chemicals
        //         .FirstOrDefaultAsync(m => m.Id == id);
        //     if (chemical == null)
        //     {
        //         return NotFound();
        //     }

        //     return View(chemical);
        // }

        // // POST: Chemicals/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int id)
        // {
        //     var chemical = await _context.Chemicals.FindAsync(id);
        //     _context.Chemicals.Remove(chemical);
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }

        private bool ChemicalExists(int id)
        {

            return _chemRepo.ChemExists(id);
            /*return _context.Chemicals.Any(e => e.Id == id);*/
        }




        //Attempt to get CID and Molecular weight from the Pug REst API
        public async Task<ExtraChemData> GetChemicalPropertiesFromPUGAPIAsync(string casNumber)
        {
            string url;          
            ExtraChemData currentData = new ExtraChemData() { CAS = casNumber };

            url = $"https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/{casNumber}/property/MolecularWeight/json";

            //attempt to get the CID and Mol Weight from pug rest
            currentData = _pug.GitCIDAndMolWeightFromPugRest(url);
             
            //If there is no cid number found by the API send back and empty extraChemData object and don't change the database
            if (currentData.CID != 0)
            {
                Chemical chem = _chemRepo.GetChemByCAS(casNumber);
                currentData = _pug.GetDensVapPresFromPUGView(currentData);
                if (await _chemRepo.TheCIDIsNullAsync(casNumber))
                {                    
                    chem.PubChemCid = currentData.CID;
                    await _chemRepo.AddOrUpdateAsync(chem);     
                }

                if (await _chemRepo.TheMolecularWeightIsNullAsync(casNumber))
                {
                    chem.MolecularWeight = currentData.MolecularWeight;
                    await _chemRepo.AddOrUpdateAsync(chem);
                }

                if (await _chemRepo.TheDensityIsNullAsync(casNumber))
                {
                    chem.Density = currentData.Density;
                    await _chemRepo.AddOrUpdateAsync(chem);
                }
                if (await _chemRepo.TheVaporPressureIsNullAsync(casNumber))
                {
                    chem.VaporPressure = currentData.VaporPressure;
                    await _chemRepo.AddOrUpdateAsync(chem);
                }
            }

            return currentData;
        }
    }
}
