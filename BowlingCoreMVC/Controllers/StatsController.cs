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

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET /Stats
        // (Highscores)
        public async Task<IActionResult> Index()
        {
            var lastweek = DateTime.Now.AddDays(-7);
            var WeeklyGames = _db.Games.Where(o => o.CreatedDate >= lastweek).OrderByDescending(o => o.Score).Take(10).ToList();
            if (WeeklyGames.Count > 0)
            {
                foreach (var g in WeeklyGames)
                {
                    var user = await _userManager.FindByIdAsync(g.UserID);
                    g.UserName = user.UserName;
                }
            }

            var AllTimeGames = _db.Games.OrderByDescending(o => o.Score).Take(10).ToList();
            if (AllTimeGames.Count > 0)
            {
                foreach (var g in AllTimeGames)
                {
                    var user = await _userManager.FindByIdAsync(g.UserID);
                    g.UserName = user.UserName;
                }
            }

            var AllTimeSeries = _db.Series.OrderByDescending(o => o.SeriesScore).Take(10).ToList();
            if (AllTimeSeries.Count > 0)
            {
                foreach (var s in AllTimeSeries)
                {
                    var user = await _userManager.FindByIdAsync(s.UserID);
                    s.UserName = user.UserName;
                }
            }

            ViewData["WeeklyGames"] = WeeklyGames;
            ViewData["AllTimeGames"] = AllTimeGames;
            ViewData["AllTimeSeries"] = AllTimeSeries;

            return View();
        }

        [Authorize]
        public async Task<IActionResult> MyStats()
        {
            var user = await GetCurrentUserAsync();

            var AllGames = _db.Games.Where(o => o.UserID == user.Id).Include(o => o.Frames).ToList();

            int SinglePinsTotal = 0;
            int SinglePinsSpares = 0;
            int TotalStrikes = 0;
            int TotalFrames = 0;
            int GamesOver200 = 0;
            int TotalPossibleStrikes = 0;
            int TotalGames = AllGames.Count();


            foreach (var g in AllGames)
            {
                if (g.Score >= 200)
                    GamesOver200++;

                TotalPossibleStrikes += 12;

                //single pin spare percentage
                foreach (var f in g.Frames)
                {
                    TotalFrames++;
                    if (f.ThrowOneScore == 9)
                    {
                        SinglePinsTotal++;
                        if (f.ThrowTwoScore == 1)
                        {
                            SinglePinsSpares++;
                        }
                    }
                    else if (f.ThrowOneScore == 10)
                    {
                        TotalStrikes++;
                    }
                }
            }

            var StatsList = new List<StatsViewModel>();
            var s1 = new StatsViewModel { StatTitle = "Single Pin Spare Percentage", Conversions = SinglePinsSpares, Total = SinglePinsTotal };
            var s2 = new StatsViewModel { StatTitle = "Strike Percentage", Conversions = TotalStrikes, Total = TotalPossibleStrikes };
            var s3 = new StatsViewModel { StatTitle = "Over 200 Games", Conversions = GamesOver200, Total = TotalGames};

            StatsList.Add(s1);
            StatsList.Add(s2);
            StatsList.Add(s3);

            return View(StatsList);
        }
    }
}