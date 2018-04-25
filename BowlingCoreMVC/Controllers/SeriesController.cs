using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using BowlingCoreMVC.Models;
using BowlingCoreMVC.Data;
using Microsoft.EntityFrameworkCore;

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
            return View(await _db.Series.ToListAsync());
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var s = _db.Series.Include(o => o.Games).Where(o => o.ID == id).SingleOrDefault();

            return View(s);
        }
    }
}