using Kartverket.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Kartverket.Models;
using Ganss.Xss;
using Microsoft.AspNetCore.SignalR;

namespace Kartverket.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserController(ILogger<UserController> logger, ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _logger = logger;
            _context = context;
            _passwordHasher = passwordHasher;
        }


        private async Task<bool> EmailExists(string Mail)
        {
            return await _context.Users.AnyAsync(u => u.Mail == Mail);
        }

        //Log-in form
        [HttpGet]
        public IActionResult LogInForm()
        {
            return View(new Login());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogInForm(Login model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Mail == model.Mail);
                //default userID is 404, default user is used for userless 'Case' registrations
                bool UserNotDefaultUser = false;
                bool UserNotCaseworkerUser = false;
                if (user != null)
                {
                    UserNotDefaultUser = user.UserID != 404;
                    UserNotCaseworkerUser = user.CaseWorkerUser == null;
                }
                //User isnt an automatically made user for caseworker, those users are not inloggable
                //due to caseworkers not needing a regular userpage
                if (user != null && UserNotDefaultUser && UserNotCaseworkerUser)
                {
                    var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName)
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

                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Feil mail eller passord.");

            }
            return View(model);
        }

        //Log-out
        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutConfirm()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        //Display of UserData
        [HttpGet]
        public async Task<IActionResult> UDOverview()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("RegistrationForm");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("RegistrationForm");
            }

            return View(user);
        }



        //Register new user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UDOverview(User model)
        {
            bool MailInModel = model.Mail != null;
            bool PasswordInModel = model.Password != null;
            if (MailInModel && PasswordInModel)
            {
                var sanitizer = new HtmlSanitizer();
                model.Mail = sanitizer.Sanitize(model.Mail);
                model.Password = sanitizer.Sanitize(model.Password);

                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Mail == model.Mail))
                {
                    ModelState.AddModelError("Mail", "Denne e-postadressen er allerede i bruk.");
                    return View("RegistrationForm", model);
                }

                Random rnd = new Random();
                int AutoUserID = rnd.Next(100000, 999999);

                var newUser = new User
                {
                    UserID = AutoUserID,
                    UserName = model.UserName,
                    Mail = model.Mail,
                    Password = _passwordHasher.HashPassword(null, model.Password),
                    CaseWorkerUser = null
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, newUser.UserID.ToString()),
            new Claim(ClaimTypes.Name, newUser.UserName)
        };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("UDOverview");
            }
            return View("RegistrationForm", model);
        }

        //Fetch UserData from database
        private async Task<User?> GetUserData()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return null;
            }
            return await _context.Users.FindAsync(userId);
        }

        [HttpGet]
        public IActionResult RegistrationForm()
        {
            return View(new User());
        }

        /// <summary>
        /// Retrieves, validates and find user in DB using userId, creates a UserUpdateModel
        /// </summary>
        /// <returns>
        /// - If the user is authenticated and found: Returns the "Profile" view with the user's information.
        /// - If user ID is invalid or not found: Redirects to the "LogInForm" action of the "Home" controller.
        /// </returns>
        /// <remarks>
        /// [ValidateAntiForgeryToken], and [Authorize] attributes
        /// protect against CSRF attacks, and restricts this to authenticated users only.
        /// </remarks> 
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("LogInForm", "Home");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("LogInForm", "Home");
            }

            var viewModel = new UserUpdateModel
            {
                UserID = user.UserID,
                UserName = user.UserName,
                Mail = user.Mail
            };

            return View(viewModel);
        }


        /// <summary>
        /// Validates UserUpdateModel, retrieves user, checks if email is inuse, 
        /// sanitizes input against XSS attacks, updates the user info 
        /// </summary>
        /// <param name="model">A UserUpdateModel object containing the updated user information.</param>
        /// <returns>
        /// - If successful: Redirects to the "Profile" view with a success message.
        /// - If model is invalid: Returns the current view with the model.
        /// - If user not found: Redirects to the "LogInForm" action of "Home" controller.
        /// - If new email already exists: Returns the view with an error message.
        /// </returns>
        /// <remarks> 
        /// [ValidateAntiForgeryToken], and [Authorize] attributes
        /// protect against CSRF attacks, and restricts this to authenticated users only.
        /// </remarks>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FindAsync(model.UserID);
            if (user == null)
            {
                return RedirectToAction("LogInForm", "Home");
            }

            // Check if the new email already exists for another user
            if (user.Mail != model.Mail)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Mail == model.Mail && u.UserID != model.UserID);
                if (emailExists)
                {
                    ModelState.AddModelError("Mail", "Denne e-postadressen er allerede i bruk.");
                    return View(model);
                }
            }


            var sanitizer = new HtmlSanitizer();

            user.UserName = sanitizer.Sanitize(model.UserName);
            user.Mail = sanitizer.Sanitize(model.Mail);
            //user.UserID = model.UserID;

            await _context.SaveChangesAsync();

            TempData["Message"] = "Profilen har blitt oppdatert!";
            return RedirectToAction("Profile");
        }
                
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Validates the PasswordUpdate Model.
        /// Finds user with userID. Verifies their current password, if verified, hashes and updates the new password.
        /// Saves changes to the database.
        /// </summary>
        /// <param name="model">A PasswordUpdateModel containing the current and new password.</param>
        /// <returns>
        /// - If successful: Redirects to the "Profile" action with a success message.
        /// - If model is invalid: Returns the current view with the model.
        /// - If user ID is invalid or user not found: Redirects to the "LogInForm" action of "Home" controller.
        /// - If current password is incorrect: Returns the view with an error message.
        /// </returns>
        /// <remarks>        
        /// [ValidateAntiForgeryToken], and [Authorize] attributes
        /// protect against CSRF attacks, and restricts this to authenticated users only.
        /// </remarks>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(PasswordUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("LogInForm", "Home");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("LogInForm", "Home");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.CurrentPassword);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Nåværende passord er ikke riktig.");
                return View(model);
            }

            user.Password = _passwordHasher.HashPassword(user, model.NewPassword);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Passordet ditt har blitt endret.";
            return RedirectToAction("Profile");
        }

        /// <summary>
        /// Retrives userID, finds and removes user from DB, signs user out of session. Gives a temp message of account deletion
        /// </summary>
        /// <returns>
        /// - If successful: Redirects to the "Index" action of "Home" controller with a success message.
        /// - If userID is invalid or not found: Redirects to the "LogInForm" action of "Home" controller.
        /// - If user not found in database: Returns a NotFound result.
        /// </returns>
        /// <remarks>
        /// [ValidateAntiForgeryToken], and [Authorize] attributes
        /// protect against CSRF attacks, and restricts this to authenticated users only.
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("LogInForm", "Home");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Message"] = "Brukeren din har nå blitt slettet.";
            return RedirectToAction("Index", "Home");
        }
    }
}