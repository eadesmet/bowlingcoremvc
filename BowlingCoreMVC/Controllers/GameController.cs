using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using BowlingCoreMVC.Data;
using BowlingCoreMVC.Models;
using BowlingCoreMVC.Helpers;
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

        [HttpGet]
        public ActionResult Create()
        {
            return View("Edit", new Game()); //TODO: Make sure this is ID 0
        }

        // GET Edit page (Edit a single game by ID)
        [HttpGet]
        public ActionResult Edit(int id)
        {
            //Old code that worked (edit a single game)
            var game = _db.Games.Include(o => o.Frames).Where(g => g.ID == id).SingleOrDefault();
            game.Frames = game.Frames.OrderBy(f => f.FrameNum).ToList();
            return View(game);

            //New 'fake' series because the view takes a series now
            //Series s = new Series();

            //var game1 = _db.Games.Include(o => o.Frames).Where(g => g.ID == id).SingleOrDefault();
            //game1.Frames = game1.Frames.OrderBy(f => f.FrameNum).ToList();

            //var game2 = _db.Games.Include(o => o.Frames).Where(g => g.ID == 2).SingleOrDefault();
            //game2.Frames = game2.Frames.OrderBy(f => f.FrameNum).ToList();

            //s.Games = new List<Game>();
            //s.Games.Add(game1);
            //s.Games.Add(game2);

            //return View(s);
        }

        [HttpPost]
        public JsonResult NextThrowClick(string JSONGame, int[] GameIDs)
        {
            Game g = JsonConvert.DeserializeObject<Game>(JSONGame);
            Game DBGame;
            int SeriesID = 0;
            if (GameIDs.Length > 1)
            {
                //return a series
                foreach (var GameID in GameIDs)
                {
                    if (g.ID == GameID)
                    {
                        //DBGame = _db.Games.Include(o => o.Frames).Where(o => o.ID == GameID).SingleOrDefault();
                        //DBGame.Frames = DBGame.Frames.OrderBy(f => f.FrameNum).ToList();
                        //SeriesID = DBGame.SeriesID;

                        //TODO: Mush together g and DBGame, score it, and save it
                        // Take the fields I want from g and set them to DBGame, and save that (g won't have everything)

                        g = ScoreHelper.ThrowCurrent(g); //this also scores the game and updates the fields

                        //DBGame = DataHelper.UpdateGame(g, DBGame); //this updates DBGame from g
                        //_db.SaveChanges();

                        //NOTE: This forces it to camelCase in javascript!!! It will not know different capitalization
                        return Json(new { jsonGameReturned = JsonConvert.SerializeObject(g) });

                    }
                }
                
            }
            else
            {
                g = ScoreHelper.ThrowCurrent(g);
                
            }
            return Json(new { jsonGameReturned = JsonConvert.SerializeObject(g) });


            //return View("Edit", g);
        }

        [HttpPost]
        public JsonResult PreviousThrowClick(string JSONGame, int[] GameIDs)
        {
            Game g = JsonConvert.DeserializeObject<Game>(JSONGame);

            g = ScoreHelper.CalculatePrevious(g);

            

            //g = DataHelper.UpdateGame(g, DBGame);

            return Json(new { jsonGameReturned = JsonConvert.SerializeObject(g) });
        }

        [HttpPost]
        public JsonResult SaveGameClick(string JSONGame, int GameID)
        {
            Game g = JsonConvert.DeserializeObject<Game>(JSONGame);
            g = ScoreHelper.ScoreGame(g);
            DataHelper.SaveGame(g, _db);

            return Json(new { jsonGameReturned = JsonConvert.SerializeObject(g) });
        }

    }
}