using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var s = _db.Series.Include(o => o.Games).Where(o => o.ID == id).SingleOrDefault();

            return View(s);
        }

        [HttpGet]
        public IActionResult Create()
        {
            Series model = new Series(); //hmmm

            //TODO: League list here needs to filter what leagues this user is in
            model.Leagues = Helpers.DataHelper.GetCurrentLeagues(_db);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Series model)
        {
            if (ModelState.IsValid)
            {
                Series s = Series.Create(model.NumberOfGames, model.LeagueID ?? 0);
                var user = await GetCurrentUserAsync();
                s = Helpers.DataHelper.CreateSeries(s, _db, user.Id);
                
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
            _db.Series.Remove(s);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}