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
                Issues = _context.Issues.ToList(),
                KommuneInfos = _context.KommuneInfo.ToList(),
                FylkesInfos = _context.FylkesInfo.ToList()
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
                    return RedirectToAction("AreaChangeOverview"); 
                }

                _context.Case.Remove(caseToDelete);
                _context.SaveChanges();

                TempData["Message"] = "Saken ble slettet.";
                return RedirectToAction("AreaChangeOverview"); 
            }
            catch (Exception ex)
            {
                // Logg eventuelle feil
                TempData["Message"] = "Det oppsto en feil under sletting av saken.";
                return RedirectToAction("AreaChangeOverview"); 
            }
        }

        [HttpPost]
        public IActionResult EditDescription(int caseId, string newDescription)
        {
            // Fetches the case baseed på CaseNo
            var caseItem = _context.Case.FirstOrDefault(c => c.CaseNo == caseId);
            if (caseItem != null)
            {
                // Updates the description
                caseItem.Description = newDescription;
                _context.SaveChanges();
            }

            // Retuns back to AreaChangeOverview. MIGHT NEED TO MAKE IT SO THAT THE ACCORDION STAYS OPEN??
            return RedirectToAction("AreaChangeOverview");
        }

    }
}
