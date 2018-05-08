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

        // GET: Leagues
        public async Task<IActionResult> Index()
        {
            //var LeaguesList = await _db.Leagues.Include(o => o.Location).ToListAsync();
            var LeaguesList = await _db.Leagues.ToListAsync();

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

            return View(league);
        }

        // GET: Leagues/Create
        public IActionResult Create()
        {
            var Model = League.Create();
            Model.Locations = Helpers.DataHelper.GetAllLocations(_db);

            //COULD put this into ViewData, maybe in the future
            //ViewData["Locations"] = ViewModel.Locations;

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

                Model.CreatedDate = DateTime.Now;
                Model.ModifiedDate = DateTime.Now;

                _db.Add(Model);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //if modelstate is invalid, get the locations again and redisplay form
            Model.Locations = Helpers.DataHelper.GetAllLocations(_db);
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

            return View(league);
        }

        // POST: Leagues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,LocationID,CreatedByID,StartDate,EndDate")] League league)
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
