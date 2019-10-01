using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index()
        {
            //if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            //    ViewData["EnvironmentTest"] = "It worked!";
            //else
            //    ViewData["EnvironmentTest"] = "nahh :(";
            var user = await GetCurrentUserAsync();
            if (user == null) 
            {
                ViewData["ContentTitle"] = "Welcome to BowlingHub! Create an Account to get started";
                return View(); 
            }

            ViewData["ContentTitle"] = "Welcome to BowlingHub!";

            // Check if there are any leagues today
            // THAT THE USER IS IN!
            DayOfWeek Today = DateTime.Today.DayOfWeek;
            int LeagueIndex = -1;
            List<League> UsersLeagues = DataHelper.UserLeagues(user.Id, _db);
            List<List<TeamLastWeekData>> UsersTeamsLastWeekSummary = new List<List<TeamLastWeekData>>();
            foreach (League l in UsersLeagues)
            {
                if (l.LeagueDay == Today)
                {
                    LeagueIndex = UsersLeagues.IndexOf(l);
                }
                List<TeamLastWeekData> data = DataHelper.GetTeamLastWeekData(l, _db, user.Id);
                if (data.Count > 0)
                    UsersTeamsLastWeekSummary.Add(data);
            }

            if (LeagueIndex != -1)
            {
                // Have we bowled it already?
                if (!_db.Series.Where(o => o.UserID == user.Id && o.LeagueID == UsersLeagues[LeagueIndex].ID && o.CreatedDate.Date == DateTime.Today).AsNoTracking().Any())
                {
                    ViewData["TodaysLeague"] = UsersLeagues[LeagueIndex];
                }
                
            }

            ListSingleValue MyGamesBowledToday = new ListSingleValue();
            MyGamesBowledToday.Title = "My Games Today";
            MyGamesBowledToday.SubTitle = DateTime.Today.ToShortDateString();
            List<Game> MyGamesToday = _db.Games.Where(o => o.UserID == user.Id && o.CreatedDate.Date == DateTime.Today).ToList();
            if (MyGamesToday != null && MyGamesToday.Count > 0)
            {
                foreach (Game g in MyGamesToday)
                {
                    MyGamesBowledToday.Keys.Add(user.UserName);
                    MyGamesBowledToday.Values.Add(g.Score);
                }
                ViewData["MyGamesBowledToday"] = MyGamesBowledToday;
            }


            // My Last 5 Games
            List<Game> Last5Games = _db.Games.Where(o => o.UserID == user.Id).OrderByDescending(o => o.CreatedDate).Take(5).ToList();
            ListSingleValue Last5GamesList = new ListSingleValue();
            Last5GamesList.Title = "My Last 5 Games";
            foreach (var g in Last5Games)
            {
                Last5GamesList.Keys.Add(g.CreatedDate.ToShortDateString());
                Last5GamesList.Values.Add(g.Score);
            }
            ViewData["MyLast5Games"] = Last5GamesList;

            // My High Games
            List<Game> MyHighGames = _db.Games.Where(o => o.UserID == user.Id).OrderByDescending(o => o.Score).Take(5).ToList();
            ListSingleValue MyHighGamesList = new ListSingleValue();
            MyHighGamesList.Title = "My High Games";
            foreach (var g in MyHighGames)
            {
                MyHighGamesList.Keys.Add(g.CreatedDate.ToShortDateString());
                MyHighGamesList.Values.Add(g.Score);
            }
            ViewData["MyHighGames"] = MyHighGamesList;

            // My High Series
            List<Series> MyHighSeries = _db.Series.Where(o => o.UserID == user.Id).OrderByDescending(o => o.SeriesScore).Take(5).ToList();
            ListSingleValue MyHighSeriesList = new ListSingleValue();
            MyHighSeriesList.Title = "My High Series";
            foreach (var s in MyHighSeries)
            {
                MyHighSeriesList.Keys.Add(s.CreatedDate.ToShortDateString());
                MyHighSeriesList.Values.Add(s.SeriesScore);
            }
            ViewData["MyHighSeries"] = MyHighSeriesList;


            // All the teams this user is a part of, in all leagues
            ViewData["AllTeamsWeekSummary"] = UsersTeamsLastWeekSummary;
            
            return View();
        }


        // Testing
        public async Task<IActionResult> QuickCreateSeries(int LeagueID)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) { RedirectToAction("Error", "User Not logged in"); }

            // I wanted to pass the whole league back to this action, but we can't
            // https://docs.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/built-in/anchor-tag-helper?view=aspnetcore-2.2#asp-route-value
            League l = _db.Leagues.Where(o => o.ID == LeagueID).SingleOrDefault();
            if (l == null) { RedirectToAction("Error", "League Not Found"); }

            // TODO(ERIC): TeamID here?
            DBOperationResult<Series> DBResult = DataHelper.CreateAndInsertSeries(_db, user.Id, l.ID);
            if (DBResult.IsError) 
            {
                HttpContext.Session.SetString("ErrorMessage", DBResult.Message);
                return RedirectToAction("Index", "Home"); //, new { Message = DBResult.Message }
            }
            
            //Series s = Series.Create(l.DefaultNumOfGames, l.ID);
            //s = DataHelper.InsertSeries(s, _db, user.Id);

            return RedirectToAction("Edit", "Series", new { id = DBResult.Item.ID });
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = Message });
        }
    }
}
