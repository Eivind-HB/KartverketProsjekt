using Kartverket.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Kartverket.Models;

namespace Kartverket.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<CaseWorker> _passwordHasher;

        public AdminController(ApplicationDbContext context, IPasswordHasher<CaseWorker> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }


        private async Task<bool> EmailExists(string Mail)
        {
            return await _context.Users.AnyAsync(e => e.Mail == Mail);
        }

        //LogIn for Admin
        [HttpGet]
        public IActionResult LogInFormAdmin()
        {
            return View(new LoginAdmin());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogInFormAdmin(LoginAdmin model)
        {
            if (ModelState.IsValid)
            {
                var allEmails = await _context.KartverketEmployee.Select(e => e.Mail).ToListAsync();
                Console.WriteLine($"All emails in database: {string.Join(", ", allEmails)}");

                // Trim the input email to remove any leading/trailing whitespace
                string trimmedInputEmail = model.Mail.Trim();

                Console.WriteLine($"Attempting login with email: '{trimmedInputEmail}'");

                var employee = await _context.KartverketEmployee
     .FirstOrDefaultAsync(e => e.Mail.Trim().Trim('\'') == trimmedInputEmail);

                if (employee == null)
                {
                    // Log lengths for debugging
                    Console.WriteLine($"Length of input email: {trimmedInputEmail.Length}");
                    var storedEmails = await _context.KartverketEmployee.Select(e => e.Mail).ToListAsync();
                    foreach (var email in storedEmails)
                    {
                        Console.WriteLine($"Stored email: '{email}', Length: {email.Length}");
                    }

                    ModelState.AddModelError(string.Empty, "Ingen ansatt funnet med denne e-postadressen.");
                    return View(model);
                }

                // Retrieve the CaseWorker record using the KartverketEmployee_EmployeeID
                var caseWorker = await _context.CaseWorkers.FirstOrDefaultAsync(cw => cw.KartverketEmployee_EmployeeID == employee.EmployeeID);

                if (caseWorker == null)
                {
                    ModelState.AddModelError(string.Empty, "Ingen saksbehandler funnet for denne ansatte.");
                    return View(model);
                }

                // Password verification logic
                bool isPasswordValid = false;

                if (caseWorker.Password == "default" && model.Password == "default")
                {
                    isPasswordValid = true;
                    // TODO: Implement logic to force password change on next login
                }
                else if (caseWorker.Password == "default")
                {
                    // If the stored password is "default" but the input isn't, it's invalid
                    isPasswordValid = false;
                }
                else
                {
                    try
                    {
                        var result = _passwordHasher.VerifyHashedPassword(caseWorker, caseWorker.Password, model.Password);
                        isPasswordValid = (result == PasswordVerificationResult.Success);
                    }
                    catch (FormatException)
                    {
                        // If we get here, the stored password is not properly hashed
                        isPasswordValid = false;
                    }
                }

                if (isPasswordValid)
                {
                    // Create claims and sign in the user
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, caseWorker.CaseWorkerID.ToString()),
                new Claim(ClaimTypes.Name, employee.Mail)
            };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // This will create a persistent cookie
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) // Cookie will expire after 7 days
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Overviewcaseworker", "Case");
                }

                // If password verification fails, return error
                ModelState.AddModelError(string.Empty, "Feil mail eller passord.");
            }
            return View(model);
        }
    }
}