using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using BowlingCoreMVC.Models;
using BowlingCoreMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BowlingCoreMVC.Controllers
{
    public class SeriesController : Controller
    {
        private ApplicationDbContext _db { get; set; }

        public SeriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var SeriesList = await _db.Series.Include(s => s.Games).ToListAsync();
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
            Models.GameViewModels.GameViewModels.SeriesViewModel model = new Models.GameViewModels.GameViewModels.SeriesViewModel();
            model.Leagues = GetCurrentLeagues();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(Models.GameViewModels.GameViewModels.SeriesViewModel model)
        {
            if (ModelState.IsValid)
            {
                Series s = Series.Create(model.NumberOfGames, model.LeagueID ?? 0);
                
                Helpers.DataHelper.CreateSeries(s, _db);
                
                return View("Edit", model);
            }
            return View(model);
        }




        //Helpers
        public List<SelectListItem> GetCurrentLeagues()
        {
            var result = new List<SelectListItem>();
            var leagues = _db.Leagues.Where(o => o.EndDate >= DateTime.Today).ToList();
            foreach (var l in leagues)
            {
                result.Add(new SelectListItem() { Value = l.ID.ToString(), Text = l.Name });
            }

            return (result);
        }
    }
}