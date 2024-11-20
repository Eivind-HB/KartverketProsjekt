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
                if (user != null)
                {
                    var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Name, user.Mail)
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

        //Display av UserData
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



        //Registrering av ny bruker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UDOverview(User model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Mail == model.Mail))
                {
                    ModelState.AddModelError("Mail", "Denne e-postadressen er allerede i bruk.");
                    return View("RegistrationForm", model);
                }

                var newUser = new User
                {
                    UserName = model.UserName,
                    Mail = model.Mail,
                    Password = _passwordHasher.HashPassword(null, model.Password)
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

        //Hent UserData fra databasen
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

        //Pofile 
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

            var viewModel = new UserUpdate
            {
                UserID = user.UserID,
                UserName = user.UserName,
                Mail = user.Mail
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserUpdate model)
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


            user.UserName = model.UserName;
            user.Mail = model.Mail;
            //user.UserID = model.UserID;

            await _context.SaveChangesAsync();

            TempData["Message"] = "Profilen har blitt oppdatert!";
            return RedirectToAction("Profile");
        }

        //Password update
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(PasswordUpdate model)
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

        //Delete user
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
