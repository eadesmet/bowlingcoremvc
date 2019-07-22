using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using BowlingCoreMVC.Models;
using BowlingCoreMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BowlingCoreMVC.Controllers
{
    [Authorize]
    public class SeriesController : Controller
    {
        private ApplicationDbContext _db { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;

        public SeriesController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var SeriesList = await _db.Series.Where(s => s.UserID == user.Id).Include(s => s.Games).ToListAsync();
            //var ViewModelList = new List<Models.GameViewModels.GameViewModels.SeriesListViewModel>();

            foreach (var s in SeriesList)
            {
                //var league = _db.Leagues.Where(o => o.ID == s.LeagueID).SingleOrDefault();
                if (s.LeagueID != null)
                {
                    var league = _db.Leagues.Where(o => o.ID == s.LeagueID).SingleOrDefault();
                    if (league != null)
                    {
                        s.LeagueName = league.Name;
                    }
                }


            }

            SeriesList = SeriesList.OrderByDescending(o => o.CreatedDate).ToList();

            return View(SeriesList);
        }

        // NOTE(Eric): Can this really be async? 
        // Might have to change it back and get the user another way
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // This eventually passes the Game ID to the EditGameViewComponent and gets the Frames there.
            // We could include the frames here, but with the way it's currently working we don't have to
            // I'm not sure which way would be better.
            //     Currently it's querying twice for the games

            //var s = _db.Series.Include(o => o.Games).Where(o => o.ID == id).SingleOrDefault();
            var s = _db.Series.Where(o => o.ID == id)
                .Include(o => o.Games).ThenInclude(g => g.Frames)
                .Include(o => o.User)
                .Include(o => o.Games).ThenInclude(g => g.User)
                .Include(o => o.League)
                .SingleOrDefault();

            //var user = await GetCurrentUserAsync();
            //foreach (var g in s.Games)
            //{
            //    g.UserName = user.UserName;
            //}

            //s.UserName = user.UserName;
            s.UserName = s.User.UserName;

            return View(s);
        }

        // GET (new) details page
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Series Result = _db.Series.Include(o => o.Games).Include(o => o.League).Include(o => o.User).Where(s => s.ID == id).SingleOrDefault();

            //var user = await GetCurrentUserAsync();
            //if (user == null) { return RedirectToAction("Login", "Account"); }

            //Result.UserName = user.UserName;

            //string LeagueName = Helpers.DataHelper.GetLeagueNameByID((int)Result.LeagueID, _db);
            //if (string.IsNullOrEmpty(LeagueName))
            //{
            //    Result.LeagueName = "No League";
            //}
            //else
            //{
            //    Result.LeagueName = LeagueName;
            //}



            return View(Result);
        }

        // Get Create Series page where user selects their League and # of games
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            Series model = new Series(); //hmmm

            var user = await GetCurrentUserAsync();

            model.Leagues = Helpers.DataHelper.GetUsersRunningLeagues(_db, user.Id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Series model)
        {
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();

                Series s = Series.Create(model.NumberOfGames, model.LeagueID ?? 0);

                int LeagueID = 0;
                if (model.LeagueID != null && model.LeagueID != 0)
                {
                    LeagueID = model.LeagueID ?? 0;

                    // @Fetch: Get the league (for use on the edit league page)
                    League l = _db.Leagues.Single(o => o.ID == LeagueID);
                    s.League = l;

                    // @Fetch: Check if the Series exists before inserting it to DB
                    DateTime LeagueNight = Helpers.DataHelper.GetNextLeagueNight(l);

                    // Don't let them create a League Series when it isn't a league night
                    if (DateTime.Today != LeagueNight)
                    {
                        HttpContext.Session.SetString("ErrorMessage", $"'{l.Name}' isn't occuring today! Next league night is on {LeagueNight.Date.ToShortDateString()}");
                        return RedirectToAction("Error", "Home");
                    }


                    if (_db.Series.Where(o => o.CreatedDate.Date == LeagueNight && o.UserID == user.Id).Any())
                    {
                        // User already created a series this league night, so exit with an error
                        HttpContext.Session.SetString("ErrorMessage", "Series already exists for this date! Only one series per League-Night is allowed!");
                        return RedirectToAction("Error", "Home");
                    }

                    // @Fetch: Infer the Team they are on, if a team exists
                    s.TeamID = Helpers.DataHelper.GetTeamIfExists((int)model.LeagueID, user.Id, _db);
                }

                s.UserName = user.UserName;
                s = Helpers.DataHelper.InsertSeries(s, _db, user.Id);

                return View("Edit", s);
            }
            return View(model);
        }

        // GET: Series/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var s = await _db.Series.Include(o => o.Games)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (s == null)
            {
                return NotFound();
            }

            var league = _db.Leagues.Where(o => o.ID == s.LeagueID).SingleOrDefault();
            if (league != null)
            {
                s.LeagueName = league.Name;
            }

            return View(s);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var s = await _db.Series.Include(o => o.Games).SingleOrDefaultAsync(m => m.ID == id);
            foreach(var g in s.Games)
            {
                _db.Games.Remove(g);
            }
            _db.Series.Remove(s);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Game");
        }
    }
}
