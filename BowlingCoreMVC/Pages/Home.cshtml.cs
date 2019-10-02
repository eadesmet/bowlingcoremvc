using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using BowlingCoreMVC.Models;
using BowlingCoreMVC.Data;
using BowlingCoreMVC.Helpers;

namespace BowlingCoreMVC.Pages
{
    public class HomeModel : PageModel
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public HomeModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        

        public async Task<IActionResult> OnGetAsync()
        {
            return Redirect("/");



            string UserID = "";
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                // Home page without user
                return Page();
            }
            ViewData["User"] = user;

            Game g = _db.Games.Take(1).SingleOrDefault();
            ViewData["Game"] = g;

            return Page();
        }
    }
}