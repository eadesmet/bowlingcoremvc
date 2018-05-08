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
            //return View(await _db.Leagues.OrderByDescending(o => o.EndDate).ToListAsync());
            var LeaguesList = await _db.Leagues.ToListAsync();
            var ViewModelList = new List<Models.GameViewModels.GameViewModels.LeagueListViewModel>();

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                ViewData["CurrentUserID"] = user.Id;
            }
            else { return View("Error"); }
            

            foreach (var l in LeaguesList)
            {
                var ViewModel = new Models.GameViewModels.GameViewModels.LeagueListViewModel();

                //Location will be required by league
                var location = _db.Locations.Where(o => o.ID == l.LocationID).SingleOrDefault();

                ViewModel.LeagueID = l.ID;
                ViewModel.LocationName = location.Name;
                ViewModel.LeagueName = l.Name;
                ViewModel.StartDate = l.StartDate;
                ViewModel.EndDate = l.EndDate;
                ViewModel.CreatedByID = l.CreatedByID;
                ViewModel.CreatedByUserName = user.UserName;
                
                ViewModelList.Add(ViewModel);
            }

            ViewModelList = ViewModelList.OrderByDescending(o => o.EndDate).ToList();
            return View(ViewModelList);
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
            var ViewModel = new Models.GameViewModels.GameViewModels.LeagueViewModel();
            ViewModel.Locations = Helpers.DataHelper.GetAllLocations(_db);

            //COULD put this into ViewData, maybe in the future
            //ViewData["Locations"] = ViewModel.Locations;

            return View(ViewModel);
        }
        
        // POST: Leagues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.GameViewModels.GameViewModels.LeagueViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                League l = League.Create();
                l.StartDate = ViewModel.StartDate;
                l.EndDate = ViewModel.EndDate;

                l.LocationID = ViewModel.LocationID;
                l.Name = ViewModel.LeagueName;

                var user = await GetCurrentUserAsync();
                l.CreatedByID = user.Id;
                
                _db.Add(l);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //if modelstate is invalid, get the locations again and redisplay form
            ViewModel.Locations = Helpers.DataHelper.GetAllLocations(_db);
            return View(ViewModel);
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
            return View(league);
        }

        // POST: Leagues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,StartDate,EndDate")] League league)
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
