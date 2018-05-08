using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using BowlingCoreMVC.Data;
using BowlingCoreMVC.Models;

namespace BowlingCoreMVC.Controllers
{
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public StatsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            //Index will be a home page of the stats sitewide
            //weekly/monthly highscores, sitewide high averages, etc..
            var lastweek = DateTime.Now.AddDays(-7);
            var WeeklyGames = _db.Games.Where(o => o.CreatedDate >= lastweek).OrderByDescending(o => o.Score).Take(10).ToList();
            foreach (var g in WeeklyGames)
            {
                var user = await _userManager.FindByIdAsync(g.UserID);
                g.UserName = user.UserName;
            }

            var AllTimeGames = _db.Games.OrderByDescending(o => o.Score).Take(10).ToList();
            foreach (var g in AllTimeGames)
            {
                var user = await _userManager.FindByIdAsync(g.UserID);
                g.UserName = user.UserName;
            }

            var AllTimeSeries = _db.Series.OrderByDescending(o => o.SeriesScore).Take(10).ToList();
            foreach (var s in AllTimeSeries)
            {
                var user = await _userManager.FindByIdAsync(s.UserID);
                s.UserName = user.UserName;
            }

            ViewData["WeeklyGames"] = WeeklyGames;
            ViewData["AllTimeGames"] = AllTimeGames;
            ViewData["AllTimeSeries"] = AllTimeSeries;

            return View();
        }

        [Authorize]
        public IActionResult MyStats()
        {

            return View();
        }
    }
}