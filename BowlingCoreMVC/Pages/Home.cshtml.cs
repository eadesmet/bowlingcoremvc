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
            string UserID = "";
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                // Home page without user
                return Page();
            }
            ViewData["User"] = user;

            // Check if there are any leagues today (That the user is IN)
            DayOfWeek Today = DateTime.Today.DayOfWeek;
            int LeagueIndex = -1;
            List<League> UsersLeagues = DataHelper.UserLeagues(UserID, _db);
            List<List<TeamLastWeekData>> UsersTeamsLastWeekSummary = new List<List<TeamLastWeekData>>();
            foreach (League l in UsersLeagues)
            {
                if (l.LeagueDay == Today)
                {
                    LeagueIndex = UsersLeagues.IndexOf(l);
                }
                List<TeamLastWeekData> data = DataHelper.GetTeamLastWeekData(l, _db, UserID);
                if (data.Count > 0)
                    UsersTeamsLastWeekSummary.Add(data);
            }

            if (LeagueIndex != -1)
            {
                // Have we bowled it already?
                if (!_db.Series.Where(o => o.UserID == UserID && o.LeagueID == UsersLeagues[LeagueIndex].ID && o.CreatedDate.Date == DateTime.Today).AsNoTracking().Any())
                {
                    ViewData["TodaysLeague"] = UsersLeagues[LeagueIndex];
                }

            }

            ListSingleValue MyGamesBowledToday = new ListSingleValue();
            MyGamesBowledToday.Title = "My Games Today";
            MyGamesBowledToday.SubTitle = DateTime.Today.ToShortDateString();
            List<Game> MyGamesToday = _db.Games.Where(o => o.UserID == UserID && o.CreatedDate.Date == DateTime.Today).ToList();
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
            List<Game> Last5Games = _db.Games.Where(o => o.UserID == UserID).OrderByDescending(o => o.CreatedDate).Take(5).ToList();
            ListSingleValue Last5GamesList = new ListSingleValue();
            Last5GamesList.Title = "My Last 5 Games";
            foreach (var g in Last5Games)
            {
                Last5GamesList.Keys.Add(g.CreatedDate.ToShortDateString());
                Last5GamesList.Values.Add(g.Score);
            }
            ViewData["MyLast5Games"] = Last5GamesList;

            // My High Games
            List<Game> MyHighGames = _db.Games.Where(o => o.UserID == UserID).OrderByDescending(o => o.Score).Take(5).ToList();
            ListSingleValue MyHighGamesList = new ListSingleValue();
            MyHighGamesList.Title = "My High Games";
            foreach (var g in MyHighGames)
            {
                MyHighGamesList.Keys.Add(g.CreatedDate.ToShortDateString());
                MyHighGamesList.Values.Add(g.Score);
            }
            ViewData["MyHighGames"] = MyHighGamesList;

            // My High Series
            List<Series> MyHighSeries = _db.Series.Where(o => o.UserID == UserID).OrderByDescending(o => o.SeriesScore).Take(5).ToList();
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

            return Page();
        }
    }
}