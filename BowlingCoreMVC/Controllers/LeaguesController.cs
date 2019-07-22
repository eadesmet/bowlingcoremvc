using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using BowlingCoreMVC.Data;
using BowlingCoreMVC.Models;
using BowlingCoreMVC.Helpers;

namespace BowlingCoreMVC.Controllers
{
    [Authorize]
    public class LeaguesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public LeaguesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        #region Request to Join
        // GET: RequestToJoin
        public IActionResult RequestToJoin(int id)
        {
            // To be ULT Model
            var ult = new UserLeagueTeam();

            ult.Leagues = DataHelper.GetAllRunningLeagues(_db);

            return View(ult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestToJoin(Models.UserLeagueTeam Model)
        {
            if (Model.LeagueID == 0)
            {
                ModelState.AddModelError("", "Please Select a League");
            }

            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();

                if (_db.UserLeagueTeams.Where(o => o.UserID == user.Id && o.LeagueID == Model.LeagueID).Any())
                {
                    // User is already a part of the team, return.
                    ModelState.AddModelError("", "You are already a part of a Team in this league");
                    Model.Leagues = DataHelper.GetAllRunningLeagues(_db);
                    return View(Model);
                }

                // Insert UserLeagueTeam
                Model.UserID = user.Id;
                DataHelper.InsertUserLeagueTeam(Model, _db);

                HttpContext.Session.SetString("SuccessMessage", "You joined the League!");

                return RedirectToAction("Index", "Home");
                // redirect
            }

            // If form was invalid, gather the Leagues again
            Model.Leagues = DataHelper.GetAllRunningLeagues(_db);
            return View(Model);
        }

        public JsonResult GetTeams(int LeagueID)
        {
            List<Team> Teams = _db.Teams.Where(o => o.LeagueID == LeagueID).ToList();
            //Teams.Insert(0, new Team() {ID = 0, TeamName="No Team"});

            return (Json(new SelectList(Teams, "ID", "TeamName")));
        }

        [HttpGet]
        public async Task<IActionResult> RequestToLeave(int id)
        {
            var user = await GetCurrentUserAsync();

            if (id != 0)
            {
                var ult = _db.UserLeagueTeams.Where(o => o.UserID == user.Id && o.TeamID == id).SingleOrDefault();
                if (ult != null)
                {
                    _db.UserLeagueTeams.Remove(ult);
                    _db.SaveChanges();
                    HttpContext.Session.SetString("SuccessMessage", "You left the Team");
                }
                else
                    HttpContext.Session.SetString("ErrorMessage", "Bad Request");
            }
            else
                HttpContext.Session.SetString("ErrorMessage", "Bad Request");


            return RedirectToAction("Index");
        }

#endregion
        // GET: Leagues
        public async Task<IActionResult> Index()
        {
            //var LeaguesList = await _db.Leagues.Include(o => o.Location).ToListAsync();
            var LeaguesList = await _db.Leagues.Where(o => o.EndDate >= DateTime.Today).ToListAsync();

            var PreviousLeagues = await _db.Leagues.Where(o => o.EndDate <= DateTime.Today).ToListAsync();

            //var ViewModelList = new List<Models.GameViewModels.GameViewModels.LeagueListViewModel>();

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                ViewData["CurrentUserID"] = user.Id;
            }
            else { return View("Error"); }

            foreach (var l in LeaguesList)
            {
                l.CreatedByUserName = (await _userManager.FindByIdAsync(l.CreatedByID)).UserName ?? "";
                if (l.LocationID != 0)
                {
                    l.Location = _db.Locations.Where(o => o.ID == l.LocationID).SingleOrDefault();
                }
            }


            foreach (var l in PreviousLeagues)
            {
                l.CreatedByUserName = (await _userManager.FindByIdAsync(l.CreatedByID)).UserName ?? "";
                if (l.LocationID != 0)
                {
                    l.Location = _db.Locations.Where(o => o.ID == l.LocationID).SingleOrDefault();
                }
            }

            ViewData["PreviousLeagues"] = PreviousLeagues;

            return View(LeaguesList);
        }

        // GET: Leagues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var league = await _db.Leagues
                .SingleOrDefaultAsync(m => m.ID == id);
            if (league == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();

            // TODO: Populate the ViewData with League information for the League summary

            // Since we don't have teams yet, let's narrow this down a bit
            // Last weeks scores for everyone
            // Season High Games + series
            // High averages

            List<Series> LastWeekSeries = new List<Series>();

            // Games per league, then narrow down by user afterwards
            List<Series> LeagueSeries = _db.Series.Include(o => o.Games).Where(o => o.LeagueID == id).ToList();

            
            if (LeagueSeries.Count == 0)
            {
                //ErrorViewModel ErrorModel = new ErrorViewModel() { Message = "Bowl some games first to see your league sheet" };
                //return View("Error", ErrorModel);
                HttpContext.Session.SetString("ErrorMessage", "Bowl some games first to see your league sheet");
            }
            
            foreach (Series s in LeagueSeries)
            {
                s.UserName = DataHelper.GetUserNameFromID(s.UserID, _db);
                foreach (Game g in s.Games)
                {
                    g.UserName = s.UserName;
                }
            }

            Game HighestLeagueGame = LeagueSeries.SelectMany(o => o.Games).OrderByDescending(o => o.Score).FirstOrDefault(); // Top 1 by default
            ViewData["HighestLeagueGame"] = HighestLeagueGame;

            Series HighestLeagueSeries = LeagueSeries.OrderByDescending(o => o.SeriesScore).FirstOrDefault();
            ViewData["HighestLeagueSeries"] = HighestLeagueSeries;

            //var s = LeagueSeries.GroupBy(o => o.UserID);

            var query = from ls in LeagueSeries
                        group ls by ls.UserID into lsGroup
                        select new ResultItem
                        {
                            UserID = lsGroup.Key,
                            UserName = DataHelper.GetUserNameFromID(lsGroup.Key, _db),
                            Average = lsGroup.SelectMany(o => o.Games).Average(o => o.Score),
                            // Could put the other queries here too.
                        };

            var LeagueAverages = query.ToList();
            ViewData["LeagueAverages"] = LeagueAverages;

            //Game TopLeagueGames = _db.Games.OrderByDescending(o => o.Score).Take(1).SingleOrDefault();

            //DayOfWeek LeagueDay = league.StartDate.DayOfWeek;

            foreach (Series s in LeagueSeries)
            {
                s.UserName = Helpers.DataHelper.GetUserNameFromID(s.UserID, _db);

                // Now + days up to the next league night
                DateTime NextLeagueNight = Helpers.DataHelper.GetNextLeagueNight(DateTime.Today);

                if ((s.CreatedDate >= NextLeagueNight.AddDays(-7))
                     && (s.CreatedDate.Date <= NextLeagueNight))
                {
                    // Series from last week
                    // Today: 12/20 (thursday)
                    // Next : 12/26 (wednesday)
                    // Scores from: 12/19 - 12/26
                    LastWeekSeries.Add(s);
                }



            }

            ViewData["LastWeekSeries"] = LastWeekSeries;

            List<Team> teams = _db.Teams.Where(o => o.LeagueID == league.ID).ToList();
            ViewData["Teams"] = teams;




            // Team Data (to be final later)
            // TODO: Clean this whole Action up

            ViewData["TeamWeekData"] = DataHelper.GetTeamLastWeekData(league.ID, _db, user.Id);


            return View(league);
        }

        // GET: Leagues/Create
        public IActionResult Create()
        {
            var Model = League.Create();
            Model.Locations = DataHelper.GetAllLocations(_db);
            Model.Days = DataHelper.GetAllDays();
            Model.Occurances = DataHelper.GetAllOccurances();

            // Select the first item by default
            Model.Occurances[0].Selected = true;

            return View(Model);
        }

        // POST: Leagues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.League Model)
        {
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                Model.CreatedByID = user.Id;

                if (Model.NewLocation && !string.IsNullOrEmpty(Model.NewLocationName))
                {
                    Location l = new Location();
                    l.CreatedByID = user.Id;
                    l.CreatedDate = DateTime.Now;
                    l.ModifiedDate = DateTime.Now;
                    l.Name = Model.NewLocationName;
                    _db.Add(l);
                    Model.LocationID = l.ID;
                }

                Model.CreatedDate = DateTime.Now;
                Model.ModifiedDate = DateTime.Now;

                Model.LeagueDay = Model.LeagueDay;

                _db.Add(Model);
                await _db.SaveChangesAsync();

                // Set the current user as an admin of the League they just created
                UserLeagueTeam ult = new UserLeagueTeam();
                ult.UserID = user.Id;
                ult.LeagueID = Model.ID;
                ult.IsActive = true;
                ult.IsAdmin = true;
                _db.Add(ult);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Get error out of modelstate.
                //IEnumerable<Microsoft.AspNetCore.Mvc.ModelBinding.ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            }

            //if modelstate is invalid, get the locations again and redisplay form
            Model.Locations = Helpers.DataHelper.GetAllLocations(_db);
            Model.Days = DataHelper.GetAllDays();
            Model.Occurances = DataHelper.GetAllOccurances();
            return View(Model);
        }

        // GET: Leagues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var league = await _db.Leagues.SingleOrDefaultAsync(m => m.ID == id);
            if (league == null)
            {
                return NotFound();
            }

            league.Locations = Helpers.DataHelper.GetAllLocations(_db);
            league.Days = DataHelper.GetAllDays();
            league.Occurances = DataHelper.GetAllOccurances();

            return View(league);
        }

        // POST: Leagues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,LocationID,CreatedByID,StartDate,EndDate,LeagueDay")] League league)
        {
            if (id != league.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                league.ModifiedDate = DateTime.Now;
                try
                {
                    _db.Update(league);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeagueExists(league.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            league.Locations = Helpers.DataHelper.GetAllLocations(_db);
            league.Days = DataHelper.GetAllDays();
            league.Occurances = DataHelper.GetAllOccurances();

            return View(league);
        }

        // GET: Leagues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var league = await _db.Leagues
                .SingleOrDefaultAsync(m => m.ID == id);
            if (league == null)
            {
                return NotFound();
            }

            return View(league);
        }

        // POST: Leagues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var league = await _db.Leagues.SingleOrDefaultAsync(m => m.ID == id);
            _db.Leagues.Remove(league);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeagueExists(int id)
        {
            return _db.Leagues.Any(e => e.ID == id);
        }
    }
}
