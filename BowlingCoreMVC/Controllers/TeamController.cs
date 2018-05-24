using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using BowlingCoreMVC.Data;
using BowlingCoreMVC.Models;


namespace BowlingCoreMVC.Controllers
{
    //[Authorize]
    public class TeamController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeamController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Team
        public async Task<IActionResult> Index()
        {
            var TeamsList = await _db.Teams.ToListAsync();

            //var user = await GetCurrentUserAsync();
            //if (user != null)
            //{
            //    ViewData["CurrentUserID"] = user.Id;
            //}
            //else { return View("Error"); }

            ViewData["CurrentUserID"] = "123";

            foreach (var t in TeamsList)
            {
                t.CreatedByUserName = (await _userManager.FindByIdAsync(t.CreatedByID)).UserName ?? "";
                if (t.LeagueID != 0)
                {
                    t.League = _db.Leagues.Where(o => o.ID == t.LeagueID).SingleOrDefault();
                }
            }
            
            return View(TeamsList);
        }

        // GET: Team/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //TODO: Change this Details page to list the users of the team, how they have done, etc.

            if (id == null)
            {
                return NotFound();
            }

            var team = await _db.Teams
                .SingleOrDefaultAsync(m => m.ID == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Team/Create
        public IActionResult Create()
        {
            var Model = Team.Create();
            Model.Leagues = Helpers.DataHelper.GetCurrentLeagues(_db);

            return View(Model);
        }

        // POST: Leagues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Team Model)
        {
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                Model.CreatedByID = user.Id;

                Model.CreatedDate = DateTime.Now;
                Model.ModifiedDate = DateTime.Now;

                _db.Add(Model);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //if modelstate is invalid, get the locations again and redisplay form
            Model.Leagues = Helpers.DataHelper.GetCurrentLeagues(_db);
            return View(Model);
        }

        // GET: Leagues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _db.Teams.SingleOrDefaultAsync(m => m.ID == id);
            if (team == null)
            {
                return NotFound();
            }

            team.Leagues = Helpers.DataHelper.GetAllLocations(_db);

            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TeamName,LeagueID,CreatedByID")] Team team)
        {
            if (id != team.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                team.ModifiedDate = DateTime.Now;
                try
                {
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
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        // GET: Team/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _db.Teams
                .SingleOrDefaultAsync(m => m.ID == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Team/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _db.Teams.SingleOrDefaultAsync(m => m.ID == id);
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
