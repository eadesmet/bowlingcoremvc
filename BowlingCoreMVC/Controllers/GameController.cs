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

        // GET Edit page (Edit a single game by ID)
        public ActionResult Edit(int id)
        {
            //Game g = _db.Games.Find(id);

            //Game g = _db.Games.Include(f => f.Frames.OrderBy(f2 => f2.FrameNum).Where(f3 => f3.GameID == id))
            //    .Where(o => o.ID == id)
            //    .SingleOrDefault();

            //var game = (from g in _db.Games.Include(o => o.Frames)
            //            where g.ID == id
            //            orderby g.Frames.OrderBy(f => f.FrameNum)
            //            select g).FirstOrDefault();

            //var game = _db.Games.Include(f => f.Frames.OrderBy(o => o.FrameNum).FirstOrDefault()).Where(x => x.ID == id).FirstOrDefault();

            var game = _db.Games.Include(o => o.Frames).Where(g => g.ID == id).SingleOrDefault();

            game.Frames = game.Frames.OrderBy(f => f.FrameNum).ToList();

            return View(game);
        }
    }
}