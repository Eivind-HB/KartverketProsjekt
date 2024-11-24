﻿using Ganss.Xss;
using Kartverket.Data;
using Kartverket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace Kartverket.Controllers
{
  
    public class CaseController : Controller
    {

        private readonly ApplicationDbContext _context;


        public CaseController(ApplicationDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Checks if the user is logged in and has a profile. If not the user is sent to the NoProfileCaseSearch View.
        /// Finds the userID and fetches only the Cases that is connected to the ID.
        /// </summary>
        /// <returns> View - HasProfileCaseOverview </returns>
        [HttpGet]
        public IActionResult HasProfileCaseOverview()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("NoProfileCaseSearch");

            var userId = int.Parse(userIdClaim.Value);

            var viewModel = new Kartverket.Models.AreaChangeOverviewModel
            {
                Cases = _context.Case.Where(c => c.User_UserID == userId).ToList(),
                Issues = _context.Issues.ToList(),
                KommuneInfos = _context.KommuneInfo.ToList(),
                FylkesInfos = _context.FylkesInfo.ToList(),
                Status = _context.Status.ToList()
            };

            return View(viewModel);
        }

        public IActionResult NoProfileCaseSearch()
        {
            return View();
        }


        [HttpGet]
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
                Employees = _context.KartverketEmployee.ToList(),
                Status = _context.Status.ToList()
            };
            return View(viewModel);
        }


        //Case Deletion
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
                    return RedirectToAction("HasProfileCaseOverview"); 
                }

                _context.Case.Remove(caseToDelete);
                _context.SaveChanges();

                TempData["Message"] = "Saken ble slettet.";
                return RedirectToAction("HasProfileCaseOverview"); 
            }
            catch (Exception ex)
            {
                // Log eventual errors
                TempData["Message"] = "Det oppsto en feil under sletting av saken.";
                return RedirectToAction("HasProfileCaseOverview"); 
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDescription(int caseId, string newDescription)
        {
            var sanitizer = new HtmlSanitizer();
            var sanitizedDescription = sanitizer.Sanitize(newDescription);

            // Fetches case based on CaseNo
            var caseItem = _context.Case.FirstOrDefault(c => c.CaseNo == caseId);
            if (caseItem != null)
            {
                // OUpdates description and saves changes
                caseItem.Description = sanitizedDescription;
                _context.SaveChanges();
            }

            // Goes back to AreaChangeOverview. MIGHT NEED TO MAKE IT SO THAT THE ACCORDION STAYS OPEN??
            return RedirectToAction("HasProfileCaseOverview");
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
            return View("NoProfileCaseSearch");
        }
        

        [HttpGet]
        public IActionResult GetCaseImage(int caseId)
        {
            var caseItem = _context.Case.FirstOrDefault(c => c.CaseNo == caseId);
            if (caseItem != null && caseItem.Images != null)
            {
                var base64 = Convert.ToBase64String(caseItem.Images);
                var imgSrc = String.Format("data:image/png;base64,{0}", base64);
                return Json(new { imgSrc });
            }
            return NotFound();
        }
    }

}
