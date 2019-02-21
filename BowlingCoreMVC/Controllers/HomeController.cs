using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BowlingCoreMVC.Models;

namespace BowlingCoreMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                ViewData["EnvironmentTest"] = "It worked!";
            else
                ViewData["EnvironmentTest"] = "nahh :(";
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
