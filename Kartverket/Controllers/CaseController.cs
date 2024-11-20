using Kartverket.Data;
using Kartverket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kartverket.Controllers
{
  
    public class CaseController : Controller
    {

        private readonly ApplicationDbContext _context;

        private static List<Case> Cases = new List<Case>();

        public CaseController(ApplicationDbContext context)
        {
            _context = context;
        }


        //Display av registrerte forandringer av terrenget
        [HttpGet]
        public IActionResult AreaChangeOverview()
        {
            var viewModel = new Kartverket.Models.AreaChangeOverviewModel
            {
                Cases = _context.Case.ToList(),
                Issues = _context.Issues.ToList(),
                KommuneInfos = _context.KommuneInfo.ToList(),
                FylkesInfos = _context.FylkesInfo.ToList(),
                Status = _context.Status.ToList()
            };
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            TempData.Remove("ErrorMessage");
            ViewBag.ViewModel = TempData["OpprettetSaksnr"];
            return View(viewModel);
        }

        public IActionResult OverviewCaseworker()
        {
            var viewModel = new Kartverket.Models.OverviewCaseworkerModel
            {
                Cases = _context.Case.ToList(),
                Issues = _context.Issues.ToList(),
                KommuneInfos = _context.KommuneInfo.ToList(),
                FylkesInfos = _context.FylkesInfo.ToList(),
                Users = _context.Users.ToList(),
                CaseWorkers = _context.CaseWorkers.ToList(),
                Employees = _context.KartverketEmployee.ToList()

            };
            return View(viewModel);
        }


        //Sletting av sak
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
        public IActionResult EditDescription(int caseId, string newDescription)
        {
            // Henter saken basert på CaseNo
            var caseItem = _context.Case.FirstOrDefault(c => c.CaseNo == caseId);
            if (caseItem != null)
            {
                // Oppdaterer beskrivelse
                caseItem.Description = newDescription;
                _context.SaveChanges();
            }

            // Går tilbake til AreaChangeOverview. MIGHT NEED TO MAKE IT SO THAT THE ACCORDION STAYS OPEN??
            return RedirectToAction("AreaChangeOverview");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CaseSearch(int CaseNo)
        {
            var cases = await _context.Case.FirstOrDefaultAsync(c => c.CaseNo == CaseNo);
            if (cases != null)
            {
                return View("CaseDetails", cases);
            }
            TempData["ErrorMessage"] = "Saken ble ikke funnet.";
            return RedirectToAction("AreaChangeOverview");
            }


        }
}
