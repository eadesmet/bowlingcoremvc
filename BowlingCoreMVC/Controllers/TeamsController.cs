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

        // GET: Teams
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _db.Teams.Include(t => t.League);
            return View(await applicationDbContext.ToListAsync());
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

            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create(int LeagueID)
        {

            // TODO(ERIC): Make this into a modal or something, it's only a name right now..

            //ViewData["LeagueID"] = new SelectList(_db.Leagues, "ID", "Name");
            Team team = new Team();
            team.LeagueID = LeagueID;

            team.Leagues = BowlingCoreMVC.Helpers.DataHelper.GetCurrentLeagues(_db);

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
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeagueID"] = new SelectList(_db.Leagues, "ID", "Name", team.LeagueID);
            return View(team);
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _db.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            ViewData["LeagueID"] = new SelectList(_db.Leagues, "ID", "Name", team.LeagueID);
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LeagueID,TeamName,CreatedByID,CreatedDate,ModifiedDate")] Team team)
        {
            if (id != team.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
            ViewData["LeagueID"] = new SelectList(_db.Leagues, "ID", "Name", team.LeagueID);
            return View(team);
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
