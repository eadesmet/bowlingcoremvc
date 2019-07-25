using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using BowlingCoreMVC.Data;
using BowlingCoreMVC.Models;

namespace BowlingCoreMVC.Controllers
{
    public class TeamsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeamsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        #region Buttons
        public IActionResult RemoveFromTeam(int TeamID, string UserID)
        {
            if (TeamID == 0 || string.IsNullOrEmpty(UserID))
                return NotFound();

            var ult = _db.UserLeagueTeams.Single(o => o.TeamID == TeamID && o.UserID == UserID);
            int LeagueID = ult.LeagueID;

            // TODO(ERIC): Check if the user is the only Admin on the team, and if they are make someone else the admin
            if (ult.IsAdmin)
            {
                var otheradmin = _db.UserLeagueTeams.Where(o => o.TeamID == TeamID && o.IsAdmin).ToList();
                if (otheradmin == null) // If there is another admin, we are fine to just continue
                {
                    // If they get here, and they didn't make someone else an admin, assign it to the next person on the team
                    var newadmin = _db.UserLeagueTeams.Where(o => o.TeamID == TeamID).FirstOrDefault();
                    if (newadmin != null)
                    {
                        newadmin.IsAdmin = true;
                        _db.SaveChanges();
                    }
                    
                }
            }

            _db.UserLeagueTeams.Remove(ult);
            _db.SaveChanges();

            return View("Index", new { id = LeagueID });
        }

        public IActionResult MakeAdmin(int TeamID, string UserID)
        {
            if (TeamID == 0 || string.IsNullOrEmpty(UserID))
                return NotFound();

            var ult = _db.UserLeagueTeams.Single(o => o.TeamID == TeamID && o.UserID == UserID);
            int LeagueID = ult.LeagueID;

            // NOTE(ERIC): Not currently enforcing that there is only 1 admin
            ult.IsAdmin = true;
            _db.SaveChanges();

            return View("Index", new { id = LeagueID });
        }

        public IActionResult RemoveAdmin(int TeamID, string UserID)
        {
            if (TeamID == 0 || string.IsNullOrEmpty(UserID))
                return NotFound();

            var ult = _db.UserLeagueTeams.Single(o => o.TeamID == TeamID && o.UserID == UserID);
            int LeagueID = ult.LeagueID;

            // NOTE(ERIC): Not currently enforcing that there is only 1 admin
            ult.IsAdmin = false;
            _db.SaveChanges();

            return View("Index", new { id = LeagueID });
        }
        #endregion

        // GET: Teams/Index/5 (LeagueID)
        public async Task<IActionResult> Index(int? id)
        {
            // NOTE(ERIC): id == LeagueID here! Showing Teams for that one League
            if (id == 0 || id == null)
                return NotFound();

            ViewData["League"] = _db.Leagues.Single(o => o.ID == id);

            var user = await GetCurrentUserAsync();
            ViewData["CurrentUserID"] = user.Id;

            // These need to be the same size, AND line up!
            var LeagueTeams = _db.Teams.AsNoTracking().Include(o => o.League).Include(o => o.UserLeagueTeams).Where(o => o.LeagueID == id).ToList();
            //var ULTs = _db.UserLeagueTeams.AsNoTracking().Where(o => o.LeagueID == id).ToList();

            //if (LeagueTeams.Count != ULTs.Count) return NotFound();

            //ViewData["TeamsAndULTs"] = LeagueTeams.Zip(ULTs, (t, u) => new { Team = t, ULT = u });

            //ViewData["ULTs"] = ULTs;

            return View(LeagueTeams);
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _db.Teams
                .Include(t => t.League)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (team == null)
            {
                return NotFound();
            }
            var user = await GetCurrentUserAsync();
            // Sample use of the new ListMultipleValue
            /*
            List<UserLeagueTeam> ULTs = _db.UserLeagueTeams.AsNoTracking().Where(o => o.TeamID == id).ToList();
            ListMultipleValue TeamMembers = new ListMultipleValue();
            TeamMembers.Title = "Team Members";
            int count = 0;
            foreach(var ult in ULTs)
            {
                TeamMembers.ColKeys.Add(new List<string>());
                TeamMembers.ColKeys[count].Add(Helpers.DataHelper.GetUserNameFromID(ult.UserID, _db));
                TeamMembers.ColKeys[count].Add("Sample");
                TeamMembers.Values.Add(123);
                count++;
            }
            ViewData["TeamMembers"] = TeamMembers;
            */

            List<UserLeagueTeam> ULTs = _db.UserLeagueTeams.AsNoTracking().Where(o => o.TeamID == id).ToList();
            List<TeamMember> TeamMembers = new List<TeamMember>();
            foreach(var ult in ULTs)
            {
                TeamMember member = new TeamMember();
                member.IsAdmin = ult.IsAdmin;
                member.UserName = Helpers.DataHelper.GetUserNameFromID(ult.UserID, _db);
                member.UserID = ult.UserID;
                TeamMembers.Add(member);

                if (ult.IsAdmin && ult.UserID == user.Id)
                    ViewData["IsCurrentUserAdmin"] = true;
            }
            ViewData["TeamMembers"] = TeamMembers;

            if (ULTs == null || ULTs.Count == 0)
                ViewData["IsCurrentUserAdmin"] = false;


            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create(int LeagueID)
        {

            // TODO(ERIC): Make this into a modal or something, it's only a name right now..

            //ViewData["LeagueID"] = new SelectList(_db.Leagues, "ID", "Name");
            Team team = new Team();
            team.LeagueID = LeagueID;

            team.Leagues = Helpers.DataHelper.GetAllRunningLeagues(_db);

            int index = team.Leagues.FindIndex(o => o.Value == LeagueID.ToString());

            team.Leagues[index].Selected = true;

            return View(team);
        }

        // POST: Teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LeagueID,TeamName,CreatedByID,CreatedDate,ModifiedDate")] Team team)
        {
            if (ModelState.IsValid)
            {
                team.CreatedDate = DateTime.Now;
                team.ModifiedDate = DateTime.Now;
                var user = await GetCurrentUserAsync();
                team.CreatedByID = user.Id;
                
                _db.Add(team);
                _db.SaveChanges();


                // NOTE(ERIC): We need a ULT record always, otherwise it would be an empty team without an Admin
                // if that happens, it can't be deleted

                // If the user isn't already a part of another team
                //if (!_db.UserLeagueTeams.Where(o => o.UserID == user.Id && o.LeagueID == team.LeagueID).Any())
                {
                    // Insert the user that's creating the team into the team when they create it.
                    // NOTE(ERIC): Maybe wouldn't want this? if a league admin is created all the teams?
                    UserLeagueTeam ult = new UserLeagueTeam();
                    ult.IsAdmin = true;
                    ult.TeamID = team.ID;
                    ult.LeagueID = team.LeagueID;
                    ult.UserID = team.CreatedByID;
                    Helpers.DataHelper.InsertUserLeagueTeam(ult, _db);
                }

                return RedirectToAction("Details", "Leagues", new { id = team.LeagueID });
            }
            ViewData["LeagueID"] = new SelectList(_db.Leagues, "ID", "Name", team.LeagueID);
            return View(team);
        }

        // GET: Teams/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var team = await _db.Teams.FindAsync(id);
        //    if (team == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["LeagueID"] = new SelectList(_db.Leagues, "ID", "Name", team.LeagueID);
        //    return View(team);
        //}

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TeamName")] Team team)
        {
            if (id != team.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string NewName = team.TeamName;
                    team = _db.Teams.Single(o => o.ID == team.ID);
                    team.TeamName = NewName;
                    team.ModifiedDate = DateTime.Now;
                    _db.Update(team);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = team.LeagueID });
            }
            ViewData["LeagueID"] = new SelectList(_db.Leagues, "ID", "Name", team.LeagueID);
            return View("Details", team);
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _db.Teams
                .Include(t => t.League)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _db.Teams.FindAsync(id);
            _db.Teams.Remove(team);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return _db.Teams.Any(e => e.ID == id);
        }
    }
}
