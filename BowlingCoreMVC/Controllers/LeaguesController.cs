using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BowlingCoreMVC.Data;
using BowlingCoreMVC.Models;

namespace BowlingCoreMVC.Controllers
{
    public class LeaguesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LeaguesController(ApplicationDbContext context)
        {
            _db = context;
        }

        // GET: Leagues
        public async Task<IActionResult> Index()
        {
            //return View(await _db.Leagues.OrderByDescending(o => o.EndDate).ToListAsync());
            var LeaguesList = await _db.Leagues.ToListAsync();
            var ViewModelList = new List<Models.GameViewModels.GameViewModels.LeagueListViewModel>();

            foreach (var l in LeaguesList)
            {
                var ViewModel = new Models.GameViewModels.GameViewModels.LeagueListViewModel();

                //Location will be required by league
                var location = _db.Locations.Where(o => o.ID == l.LocationID).SingleOrDefault();

                ViewModel.LocationName = location.Name;
                ViewModel.LeagueName = l.Name;
                ViewModel.StartDate = l.StartDate;
                ViewModel.EndDate = l.EndDate;
                
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
            //league.CreatedByID = ; //TODO: Logged in User
            

            if (ModelState.IsValid)
            {
                League l = League.Create();
                l.StartDate = ViewModel.StartDate;
                l.EndDate = ViewModel.EndDate;

                l.LocationID = ViewModel.LocationID;
                l.Name = ViewModel.LeagueName;
                
                _db.Add(l);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

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
        public async Task<IActionResult> Edit(int id, [Bind("Name,StartDate,EndDate")] League league)
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
