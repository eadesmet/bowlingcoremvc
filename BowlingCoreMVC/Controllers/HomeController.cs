using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

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
            if (user == null) { return View(); }

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
                UsersTeamsLastWeekSummary.Add(DataHelper.GetTeamLastWeekData(l.ID, _db, user.Id));
            }

            if (LeagueIndex != -1)
            {
                ViewData["TodaysLeague"] = UsersLeagues[LeagueIndex];
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


            /*
            ViewData["ListSingleValue"] = new ListSingleValue()
            {
                Title = "Test Title",
                SubTitle = "Subtitle here",
                Keys = new List<string>() { "bowler 1", "bowler 2" },
                Values = new List<int>() { 123, 234 }
            };
            
            TamLastWeekData data = new TeamLastWeekData()
            {
                TeamName = "Team Name",
                SubTitle = "subtitle",
                UserNames = new List<string>() { "user 1", "user 2" },
                Averages = new List<double>() { 123, 234 },
                TotalGames = new List<int>() { 12, 12 },
                TotalPins = new List<int>() { 400, 400 },
                Series = DataHelper.GetAllSeriesByUserID(user.Id.ToString(), _db).ToList()
            };
            */

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
