using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SelectPdf;
using SpillTracker.Models;
using SpillTracker.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpillTracker.Controllers
{
    [Route("pdf")]
    public class PDFController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISpillTrackerStuserFacilityRepository _stUserFacRepo;
        private readonly ISpillTrackerFormRepository _FormRepo;
        public PDFController(ISpillTrackerStuserFacilityRepository stUserFacRepo, ISpillTrackerFormRepository FormRepo, UserManager<IdentityUser> userManager)
        {
            _FormRepo = FormRepo;
            _stUserFacRepo = stUserFacRepo;
            _userManager = userManager;
        }
      
        [AllowAnonymous]
        // GET: Generate PDF Report    
        public async Task<IActionResult> Index(int id)
        {
            Stuser theFacilityManager = new Stuser();
            Form currentForm = await _FormRepo.GetFormByIdReturnAllFields(id);

            //Find the Facility Manager of the facility that had the spill
            IQueryable<StuserFacility> users = _stUserFacRepo.GetAllUsersByFacilityId((int)currentForm.FacilityId);
            foreach(StuserFacility mb in users)
            {                
                var sameUser = _userManager.Users.Where(u => u.Id == mb.Stuser.AspnetIdentityId).FirstOrDefault();
                if (await _userManager.IsInRoleAsync(sameUser, "FacilityManager"))
                {
                    theFacilityManager = mb.Stuser;
                }               
            }

            if (currentForm == null)
            {
                return NotFound();
            }

            //store items in printresultsvm
            PrintResult pr = new PrintResult();
            PrintResultsVM prvm = new PrintResultsVM();
            pr.theFormToPrint = currentForm;
            pr.theFacilityManager = theFacilityManager;
            prvm.Results = pr;

            return View(prvm);
        }

        [Route("PDFReport")]
        public async Task<IActionResult> PDFReport(int id)
        {
            var user = await _userManager.GetUserAsync(User);          
            /*var mobileView = new HtmlToPdf();
            mobileView.Options.WebPageWidth = 480;*/
            var tabletView = new HtmlToPdf();

            // create a new pdf document
            PdfDocument doc = tabletView.ConvertUrl(string.Format("{0}://{1}",
                        HttpContext.Request.Scheme, HttpContext.Request.Host) + $"/pdf?id={id}");
           
            // set the pdf viewer preferences
            doc.ViewerPreferences.CenterWindow = true;
            doc.ViewerPreferences.DisplayDocTitle = true;
            doc.ViewerPreferences.FitWindow = true;
            doc.ViewerPreferences.HideMenuBar = true;
            doc.ViewerPreferences.HideToolbar = true;
            doc.ViewerPreferences.HideWindowUI = false;
            doc.ViewerPreferences.NonFullScreenPageMode =
                PdfViewerFullScreenExitMode.UseNone;
            doc.ViewerPreferences.PageLayout =
                PdfViewerPageLayout.SinglePage;
            doc.ViewerPreferences.PageMode =
                PdfViewerPageMode.UseNone;

            // save pdf document
            byte[] pdfBytes = doc.Save();

            // close pdf document
            doc.Close();

            // return resulted pdf document         
            return File(pdfBytes, "application/pdf");

        }
    }
}
