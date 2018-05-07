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
            var ViewModelList = new List<Models.GameViewModels.GameViewModels.SeriesListViewModel>();

            foreach (var s in SeriesList)
            {
                var ViewModel = new Models.GameViewModels.GameViewModels.SeriesListViewModel();

                ViewModel.SeriesID = s.ID;
                var league = _db.Leagues.Where(o => o.ID == s.LeagueID).SingleOrDefault();
                if (league != null)
                {
                    ViewModel.LeagueName = league.Name;
                }
                
                ViewModel.BowlDate = s.CreatedDate;
                ViewModel.SeriesScore = s.SeriesScore;
                ViewModel.Games = s.Games.ToList();
                

                ViewModelList.Add(ViewModel);
            }

            ViewModelList = ViewModelList.OrderByDescending(o => o.BowlDate).ToList();

            return View(ViewModelList);
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

            //TODO: League list here needs to filter what leagues this user is in
            model.Leagues = Helpers.DataHelper.GetCurrentLeagues(_db);
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(Models.GameViewModels.GameViewModels.SeriesViewModel model)
        {
            if (ModelState.IsValid)
            {
                Series s = Series.Create(model.NumberOfGames, model.LeagueID ?? 0);
                
                Helpers.DataHelper.CreateSeries(s, _db);
                
                return View("Edit", s);
            }
            return View(model);
        }




        //Helpers
        //public List<SelectListItem> GetCurrentLeagues()
        //{
        //    var result = new List<SelectListItem>();
        //    var leagues = _db.Leagues.Where(o => o.EndDate >= DateTime.Today).ToList();
        //    foreach (var l in leagues)
        //    {
        //        result.Add(new SelectListItem() { Value = l.ID.ToString(), Text = l.Name });
        //    }

        //    return (result);
        //}

        //public List<SelectListItem> GetAllLocations()
        //{
        //    var result = new List<SelectListItem>();
        //    var locations = _db.Locations.ToList();
        //    foreach (var l in locations)
        //    {
        //        result.Add(new SelectListItem() { Value = l.ID.ToString(), Text = l.Name });
        //    }

        //    return (result);
        //}
    }
}