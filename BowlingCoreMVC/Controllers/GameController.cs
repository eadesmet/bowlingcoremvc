using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using BowlingCoreMVC.Data;
using BowlingCoreMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BowlingCoreMVC.Controllers
{
    public class GameController : Controller
    {
        private ApplicationDbContext _db { get; set; }

        public GameController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET Index (game list page)
        public async Task<IActionResult> Index()
        {
            //TODO: Filter by user. group/filter by series?
            return View(await _db.Games.ToListAsync());
        }
    }
}