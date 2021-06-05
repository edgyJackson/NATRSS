using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpillTracker.Controllers
{
    public class PDFController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
