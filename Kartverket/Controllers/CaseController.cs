using Kartverket.Data;
using Kartverket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kartverket.Controllers
{
  
    public class CaseController : Controller
    {

        private readonly ApplicationDbContext _context;

        public CaseController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult AreaChangeOverview()
        {
            var viewModel = new Kartverket.Models.AreaChangeOverviewModel
            {
                Cases = _context.Case.ToList(),
                Issues = _context.Issues.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult DeleteCase(int caseId)
        {
            try
            {
                var caseToDelete = _context.Case.FirstOrDefault(c => c.CaseNo == caseId);
                if (caseToDelete == null)
                {
                    TempData["Message"] = "Saken ble ikke funnet.";
                    return RedirectToAction("AreaChangeOverview"); // Gå tilbake til oversiktsiden
                }

                _context.Case.Remove(caseToDelete);
                _context.SaveChanges();

                TempData["Message"] = "Saken ble slettet.";
                return RedirectToAction("AreaChangeOverview"); // Gå tilbake til oversiktsiden
            }
            catch (Exception ex)
            {
                // Logg eventuelle feil
                TempData["Message"] = "Det oppsto en feil under sletting av saken.";
                return RedirectToAction("AreaChangeOverview"); // Gå tilbake til oversiktsiden
            }
        }

    }
}
