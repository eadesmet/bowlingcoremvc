using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using BowlingCoreMVC.Data;
using BowlingCoreMVC.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

        // GET Edit page (Edit a single game by ID)
        public ActionResult Edit(int id)
        {
            //Old code that worked (edit a single game)
            //var game = _db.Games.Include(o => o.Frames).Where(g => g.ID == id).SingleOrDefault();
            //game.Frames = game.Frames.OrderBy(f => f.FrameNum).ToList();
            //return View(game);

            //New 'fake' series because the view takes a series now
            Series s = new Series();

            var game1 = _db.Games.Include(o => o.Frames).Where(g => g.ID == id).SingleOrDefault();
            game1.Frames = game1.Frames.OrderBy(f => f.FrameNum).ToList();

            var game2 = _db.Games.Include(o => o.Frames).Where(g => g.ID == 2).SingleOrDefault();
            game2.Frames = game2.Frames.OrderBy(f => f.FrameNum).ToList();

            s.Games = new List<Game>();
            s.Games.Add(game1);
            s.Games.Add(game2);

            return View(s);
        }

        [HttpPost]
        public ActionResult NextThrowClick(string JSONGame)
        {
            Game g = JsonConvert.DeserializeObject<Game>(JSONGame);


            //This won't work, it needs to be a series.
            return View("Edit", g);
        }
    }
}