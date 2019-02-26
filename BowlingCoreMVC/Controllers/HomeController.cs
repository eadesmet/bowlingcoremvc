using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using BowlingCoreMVC.Models;
using BowlingCoreMVC.Data;
using BowlingCoreMVC.Helpers;

namespace BowlingCoreMVC.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _db { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;


        public HomeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public IActionResult Index()
        {
            //if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            //    ViewData["EnvironmentTest"] = "It worked!";
            //else
            //    ViewData["EnvironmentTest"] = "nahh :(";
            var user = GetCurrentUserAsync();
            if (user == null) { return View(); }

            // Check if there are any leagues today
            // THAT THE USER IS IN!
            

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "So, what is this place?";

            return View();
        }

        //public IActionResult Contact()
        //{
        //    ViewData["Message"] = "Your contact page.";

        //    return View();
        //}

        public IActionResult Error(string Message = "")
        {
            // TODO(ERIC): Better error handling; I don't want the message in the URL
            ViewData["Message"] = Message;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
