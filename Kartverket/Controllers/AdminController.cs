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
        //LogIn for Admin
        [HttpGet]
        public IActionResult LogInFormAdmin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogInFormAdmin(Login model)
        {
            if (ModelState.IsValid)
            {
            }
            return RedirectToAction("OverviewCaseworker", "Case");
        }
    }
}
