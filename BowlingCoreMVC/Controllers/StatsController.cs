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

        private const Int16 MISSED_10 = 512;   // 0000 0010 0000 0000
        private const Int16 MISSED_9 = 256;    // 0000 0001 0000 0000
        private const Int16 MISSED_8 = 128;    // 0000 0000 1000 0000
        private const Int16 MISSED_7 = 64;     // 0000 0000 0100 0000
        private const Int16 MISSED_6 = 32;     // 0000 0000 0010 0000
        private const Int16 MISSED_5 = 16;     // 0000 0000 0001 0000
        private const Int16 MISSED_4 = 8;      // 0000 0000 0000 1000
        private const Int16 MISSED_3 = 4;      // 0000 0000 0000 0100
        private const Int16 MISSED_2 = 2;      // 0000 0000 0000 0010
        private const Int16 MISSED_1 = 1;      // 0000 0000 0000 0001
        private const Int16 MISSED_0 = 0;      // 0000 0000 0000 0000
        private const Int16 MISSED_ALL = 1023; // 0000 0011 1111 1111

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
                    //if (g.UserName.ToUpper() == "TEST")
                    //{
                    //    WeeklyGames.Remove(g);
                    //}
                }
            }

            var AllTimeGames = _db.Games.OrderByDescending(o => o.Score).Take(10).ToList();
            if (AllTimeGames.Count > 0)
            {
                foreach (var g in AllTimeGames)
                {
                    var user = await _userManager.FindByIdAsync(g.UserID);
                    g.UserName = user.UserName;
                    //if (g.UserName.ToUpper() == "TEST")
                    //{
                    //    AllTimeGames.Remove(g);
                    //}
                }
            }

            var AllTimeSeries = _db.Series.OrderByDescending(o => o.SeriesScore).Take(10).ToList();
            if (AllTimeSeries.Count > 0)
            {
                foreach (var s in AllTimeSeries)
                {
                    var user = await _userManager.FindByIdAsync(s.UserID);
                    s.UserName = user.UserName;
                    //if (s.UserName.ToUpper() == "TEST")
                    //{
                    //    AllTimeSeries.Remove(s);
                    //}
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
            int TenPinSpares = 0;
            int TenPinConversions = 0;
            int PossibleSpares = 0;
            int SpareConversions = 0;

            int Over600Series = 0;

            
            foreach (var g in AllGames)
            {
                if (g.Score >= 200)
                    GamesOver200++;

                TotalPossibleStrikes += 12;

                foreach (var f in g.Frames)
                {
                    TotalFrames++;

                    // Single pin spares
                    if (f.ThrowOneScore == 9)
                    {
                        SinglePinsTotal++;
                        if (f.ThrowTwoScore == 1)
                        {
                            SinglePinsSpares++;
                        }

                        if (f.ThrowOnePins == MISSED_10)
                        {
                            TenPinSpares++;
                            if (f.ThrowTwoPins == MISSED_0)
                            {
                                TenPinConversions++;
                            }
                        }
                    }
                    else if (f.ThrowOneScore == 10)
                    {
                        TotalStrikes++;
                        if (f.FrameNum == 10 && f.ThrowTwoScore != 10)
                        {
                            // Can only get 1 spare in the 10th frame
                            // but it might be from the second + third throw
                            PossibleSpares++;
                        }
                    }

                    if (f.ThrowOneScore != 10)
                    {
                        PossibleSpares++;
                        
                        if (f.ThrowOneScore + f.ThrowTwoScore == 10)
                        {
                            SpareConversions++;
                        }
                    }
                }
            }

            List<Series> AllSeries = _db.Series.Where(o => o.UserID == user.Id).ToList();

            foreach (Series s in AllSeries)
            {
                if (s.SeriesScore >= 600)
                    Over600Series++;
            }

            var StatsList = new List<StatsViewModel>();
            var s2 = new StatsViewModel { StatTitle = "Strike Percentage", Conversions = TotalStrikes, Total = TotalPossibleStrikes };
            StatsList.Add(s2);
            var s5 = new StatsViewModel { StatTitle = "Spare Percentage", Conversions = SpareConversions, Total = PossibleSpares };
            StatsList.Add(s5);
            var s1 = new StatsViewModel { StatTitle = "Single Pin Spare Percentage", Conversions = SinglePinsSpares, Total = SinglePinsTotal };
            StatsList.Add(s1);
            var s4 = new StatsViewModel { StatTitle = "10 Pin Spares", Conversions = TenPinConversions, Total = TenPinSpares };
            StatsList.Add(s4);
            var s3 = new StatsViewModel { StatTitle = "Over 200 Games", Conversions = GamesOver200, Total = TotalGames };
            StatsList.Add(s3);
            var s6 = new StatsViewModel { StatTitle = "Over 600 Series", Conversions = Over600Series, Total = AllSeries.Count };
            StatsList.Add(s6);
            


            return View(StatsList);
        }

        // NOTE: Might not want to have the UserID in the URL here.. Username instead?
        // TODO: Make this efficient
        public async Task<IActionResult> UserSummary(string id)
        {
            if (id == null)
                RedirectToAction("Index");

            if (!_db.Games.Where(o => o.UserID == id).Any())
                RedirectToAction("Index");

            var user = await _userManager.FindByIdAsync(id);
            ViewData["UserName"] = user.UserName;

            var AllUsersGames = _db.Games.Include(o => o.Frames).Where(o => o.UserID == id).ToList();
            var AllUsersSeries = _db.Series.Where(o => o.UserID == id).ToList();

            var Last10Games = AllUsersGames.OrderByDescending(o => o.CreatedDate).Take(10).ToList();
            ViewData["Last10Games"] = Last10Games;

            double UsersAverage = AllUsersGames.Sum(o => o.Score) / AllUsersGames.Count;
            ViewData["OverallAverage"] = UsersAverage;

            var Top5Games = AllUsersGames.OrderByDescending(o => o.Score).Take(5).ToList();
            ViewData["Top5Games"] = Top5Games;

            var Top5Series = AllUsersSeries.OrderByDescending(o => o.SeriesScore).Take(5).ToList();
            ViewData["Top5Series"] = Top5Series;

            List<double> UsersLeaguesAverages = new List<double>();
            List<League> UsersLeagues = Helpers.DataHelper.UserLeagues(id, _db);
            foreach (League l in UsersLeagues)
            {
                UsersLeaguesAverages.Add(Helpers.DataHelper.UsersLeagueAverage(id, l.ID, _db));
            }

            ViewData["UsersLeagues"] = UsersLeagues;
            ViewData["UsersLeaguesAverages"] = UsersLeaguesAverages;

            //TODO: These stats need to account for the 10th frame.... :(
            double TotalFrames = 0.0, TotalSpares = 0.0, TotalStrikes = 0.0, TotalSpareFrames = 0.0;
            foreach (Game g in AllUsersGames)
            {
                foreach (var f in g.Frames)
                {
                    TotalFrames++;
                    if ((f.ThrowOneScore + f.ThrowTwoScore == 10) && (f.ThrowOneScore != 10))
                    {
                        TotalSpares++;
                    }

                    if (f.ThrowOneScore != 10)
                    {
                        TotalSpareFrames++;
                    }

                    if (f.ThrowOneScore == 10)
                    {
                        TotalStrikes++;
                    }
                }
            }

            ViewData["OverallStrikePerc"] = TotalStrikes / TotalFrames;
            ViewData["OverallSparePerc"] = TotalSpares / TotalSpareFrames;



            int SinglePinsTotal = 0;
            int SinglePinsSpares = 0;
//            int TotalStrikes = 0;
//            int TotalFrames = 0;
            int GamesOver200 = 0;
            int TotalPossibleStrikes = 0;
            int TotalGames = AllUsersGames.Count();
            int TenPinSpares = 0;
            int TenPinConversions = 0;
            int PossibleSpares = 0;
            int SpareConversions = 0;

            int Over600Series = 0;

            
            foreach (var g in AllUsersGames)
            {
                if (g.Score >= 200)
                    GamesOver200++;

                TotalPossibleStrikes += 12;

                foreach (var f in g.Frames)
                {
                    TotalFrames++;

                    // Single pin spares
                    if (f.ThrowOneScore == 9)
                    {
                        SinglePinsTotal++;
                        if (f.ThrowTwoScore == 1)
                        {
                            SinglePinsSpares++;
                        }

                        if (f.ThrowOnePins == MISSED_10)
                        {
                            TenPinSpares++;
                            if (f.ThrowTwoPins == MISSED_0)
                            {
                                TenPinConversions++;
                            }
                        }
                    }
                    else if (f.ThrowOneScore == 10)
                    {
                        TotalStrikes++;
                        if (f.FrameNum == 10 && f.ThrowTwoScore != 10)
                        {
                            // Can only get 1 spare in the 10th frame
                            // but it might be from the second + third throw
                            PossibleSpares++;
                        }
                    }

                    if (f.ThrowOneScore != 10)
                    {
                        PossibleSpares++;
                        
                        if (f.ThrowOneScore + f.ThrowTwoScore == 10)
                        {
                            SpareConversions++;
                        }
                    }
                }
            }

            foreach (Series s in AllUsersSeries)
            {
                if (s.SeriesScore >= 600)
                    Over600Series++;
            }

            var StatsList = new List<StatsViewModel>();
            var s2 = new StatsViewModel { StatTitle = "Strike Percentage", Conversions = TotalStrikes, Total = TotalPossibleStrikes };
            StatsList.Add(s2);
            var s5 = new StatsViewModel { StatTitle = "Spare Percentage", Conversions = SpareConversions, Total = PossibleSpares };
            StatsList.Add(s5);
            var s1 = new StatsViewModel { StatTitle = "Single Pin Spare Percentage", Conversions = SinglePinsSpares, Total = SinglePinsTotal };
            StatsList.Add(s1);
            var s4 = new StatsViewModel { StatTitle = "10 Pin Spares", Conversions = TenPinConversions, Total = TenPinSpares };
            StatsList.Add(s4);
            var s3 = new StatsViewModel { StatTitle = "Over 200 Games", Conversions = GamesOver200, Total = TotalGames };
            StatsList.Add(s3);
            var s6 = new StatsViewModel { StatTitle = "Over 600 Series", Conversions = Over600Series, Total = AllUsersSeries.Count };
            StatsList.Add(s6);

            ViewData["StatsList"] = StatsList;


            
            
            return View();
        }
    }
}
