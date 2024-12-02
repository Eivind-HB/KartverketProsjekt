using Ganss.Xss;
using Kartverket.Data;
using Kartverket.Models.ModelsDB;
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

            var viewModel = new Kartverket.Models.ViewModels.MultipleCasesModel
            {
                Cases = _context.Case
                .Include(c => c.KommuneInfo) // Include KommuneInfo
                .Include(c => c.Status) // Include KommuneInfo
                .Include(c => c.FylkesInfo) // Include KommuneInfo
                .Include(c => c.Issue) // Include KommuneInfo
                .Where(c => c.User_UserID == userId).ToList(),
                AllIssues = _context.Issues.ToList()

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
            var viewModel = new Kartverket.Models.ViewModels.OverviewCaseworkerModel
            {
                Cases = _context.Case
                .Include(c => c.KommuneInfo) // Include KommuneInfo
                .Include(c => c.Status) // Include KommuneInfo
                .Include(c => c.FylkesInfo) // Include KommuneInfo
                .Include(c => c.Issue).ToList(), // Include KommuneInfo
                AllIssues = _context.Issues.ToList(),
                AllStatus = _context.Status.ToList(),
                Users = _context.Users
                .Include(c => c.Cases).ToList(),
                CaseWorkers = _context.CaseWorkers.ToList(),
                Employees = _context.KartverketEmployee
                .Include(c => c.CaseWorkers).ToList()
            };
            return View(viewModel);
        }
                

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

        /// <summary>        
        /// Edits existing case by updating type, status and description.
        /// Sanatizes description from XSS attacks        
        /// </summary>
        /// <param name="caseId">The caseID of the case you want to be edited.</param>
        /// <param name="newIssueType">The new issue type you want to assing the case.</param>
        /// <param name="newStatus">The new status you want to give to the case.</param>
        /// <param name="newDescription">The new description text for the case.</param>
        /// <returns>
        /// - If the case is successfully updated: Returns a JSON object with the updated case information.
        /// - If the case is not found: Returns a BadRequest result.
        /// </returns>
        /// <remarks>  
        /// [ValidateAntiForgeryToken] attributes protects against CSRF attacks.
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCase(int caseId, int newIssueType, int newStatus, string newDescription, string newComment)
        {
            var sanitizer = new HtmlSanitizer();
            var sanitizedDescription = sanitizer.Sanitize(newDescription);
            var sanitizedComment = sanitizer.Sanitize(newComment);

            // Fetches case based on CaseNo
            var caseItem = _context.Case.FirstOrDefault(c => c.CaseNo == caseId);
            if (caseItem != null)
            {
                // Updates issue type, description, status, and comment, then saves changes
                caseItem.IssueNo = newIssueType;
                caseItem.Description = sanitizedDescription;
                caseItem.StatusNo = newStatus;
                caseItem.CommentCaseWorker = sanitizedComment;
                _context.SaveChanges();

                var issueTypeName = _context.Issues.FirstOrDefault(i => i.issueNo == newIssueType)?.IssueType;
                var statusName = _context.Status.FirstOrDefault(s => s.StatusNo == newStatus)?.StatusName;

                return Json(new { caseId, newIssueTypeName = issueTypeName, newDescription = sanitizedDescription, newStatusName = statusName, newComment = sanitizedComment });
            }

            return BadRequest();
        }


        /// <summary>
        /// Searches for case, makes a singleCaseModel        
        /// </summary>
        /// <param name="CaseNo"> The case number of the wanted case.</param>
        /// <returns>
        /// - If the case is found: Returns the "CaseDetails" view populated with the case information and related data.
        /// - If the case is not found: Sets an error message in TempData and returns the "NoProfileCaseSearch" view.
        /// </returns>
        /// <remarks> 
        /// [ValidateAntiForgeryToken] attributes protects against CSRF attacks.       
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CaseSearch(int CaseNo)
        {
            // Retrieve a single case with related data
            var singleCase = await _context.Case
                .Include(c => c.KommuneInfo)  // Include KommuneInfo
                .Include(c => c.Status)        // Include Status
                .Include(c => c.FylkesInfo)    // Include FylkesInfo
                .Include(c => c.Issue)         // Include Issue
                .FirstOrDefaultAsync(c => c.CaseNo == CaseNo); // Filter by CaseNo

            if (singleCase != null)
            {
                // Create the view model for a single case
                var viewModel = new Kartverket.Models.ViewModels.SingleCaseModel
                {
                    Case = singleCase // Assign the retrieved case directly
                };

                return View("CaseDetails", viewModel);
            }

            TempData["ErrorMessage"] = "Saken ble ikke funnet.";
            return View("NoProfileCaseSearch");
        }


        /// <summary>
        /// Checks database if case exists and if it has an image. Converts image to base64 string      
        /// </summary>
        /// <param name="caseId">The caseId which the image is requested.</param>
        /// <returns>
        /// - If the case is found and has an associated image: Returns a JSON object containing the base64-encoded image source.
        /// - If the case is not found or does not have an associated image: Returns a NotFound result.
        /// </returns>        
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

        [HttpPost]
        public IActionResult AssignCaseworker(int caseNo, int caseworkerID, decimal paidHours)
        {
            // Check if the assignment already exists
            var existingAssignment = _context.CaseWorkerAssignment
                .FirstOrDefault(c => c.CaseNo == caseNo && c.CaseWorkerID == caseworkerID);

            if (existingAssignment == null)
            {
                var caseWorkerAssignment = new CaseWorkerAssignment
                {
                    CaseNo = caseNo,
                    CaseWorkerID = caseworkerID,
                    PaidHours = paidHours
                };

                _context.CaseWorkerAssignment.Add(caseWorkerAssignment);
                _context.SaveChanges();
            }

            return Json(new { caseNo, caseworkerID });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCaseUser(int caseId, string newDescription, int IssueNo)
        {
            // Check if the IssueNo exists in the Issues table
            var issueExists = _context.Issues.Any(i => i.issueNo == IssueNo);
            if (!issueExists)
            {
                // Return an error message if the IssueNo does not exist
                TempData["Message"] = "Ugyldig IssueNo.";
                return RedirectToAction("HasProfileCaseOverview");
            }

            // Fetch the case from the database
            var caseToUpdate = _context.Case.Find(caseId);
            if (caseToUpdate == null)
            {
                return NotFound();
            }

            // Update the case properties
            caseToUpdate.Description = newDescription;
            caseToUpdate.IssueNo = IssueNo;

            // Save the changes to the database
            _context.SaveChanges();

            return RedirectToAction("HasProfileCaseOverview");
        }
    }
}
