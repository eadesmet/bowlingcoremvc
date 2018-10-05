using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using BowlingCoreMVC.Data;
using BowlingCoreMVC.Models;
using BowlingCoreMVC.Helpers;


namespace BowlingCoreMVC.Controllers
{
    //[Authorize]
    public class GameController : Controller
    {
        private ApplicationDbContext _db { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        

        public GameController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET Index (game list page)
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) { return RedirectToAction("Login", "Account"); }

            List<Game> GamesList = await _db.Games.Where(o => o.UserID == user.Id).OrderByDescending(o => o.CreatedDate).ToListAsync();
            //List<Game> GamesList = DataHelper.GetNonSeriesGamesByUserID(user.Id, _db);

            ViewData["UserSeries"] = DataHelper.GetAllSeriesByUserID(user.Id, _db);

            return View(GamesList);
        }

        // GET Create (Create a new game, redirect to Edit page)
        //[HttpGet]
        //public ActionResult Create()
        //{
        //    Game g = Game.Create();
        //    return View("Edit", g);
        //}

        // GET Edit page (Edit a single game by ID)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Game game;
            if (id == 0)
            {
                game = Game.Create();
            }
            else
            {
                game = _db.Games.Include(o => o.Frames).Where(g => g.ID == id).SingleOrDefault();
            }
            
            game.Frames = game.Frames.OrderBy(f => f.FrameNum).ToList();

            var user = await GetCurrentUserAsync();
            if (user == null) { return RedirectToAction("Login", "Account"); }

            game.UserName = user.UserName;

            return View(game);
        }

        // GET (new) details page
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var game = _db.Games.Include(o => o.Frames).Where(g => g.ID == id).SingleOrDefault();
            game.Frames = game.Frames.OrderBy(f => f.FrameNum).ToList();

            var user = await GetCurrentUserAsync();
            if (user == null) { return RedirectToAction("Login", "Account"); }

            game.UserName = user.UserName;

            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _db.Games
                .SingleOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game= await _db.Games.SingleOrDefaultAsync(m => m.ID == id);
            _db.Games.Remove(game);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        #region "editgame buttons"
        // Next Throw buton handler
        [HttpPost]
        public JsonResult NextThrowClick(string JSONGame, int[] GameIDs)
        {
            Game g = JsonConvert.DeserializeObject<Game>(JSONGame);
            
            g = ScoreHelper.ThrowCurrent(g);

            return Json(new { jsonGameReturned = JsonConvert.SerializeObject(g) });
        }

        // Previous throw button handler
        [HttpPost]
        public JsonResult PreviousThrowClick(string JSONGame, int[] GameIDs)
        {
            Game g = JsonConvert.DeserializeObject<Game>(JSONGame);

            g = ScoreHelper.CalculatePrevious(g);

            return Json(new { jsonGameReturned = JsonConvert.SerializeObject(g) });
        }

        // Save game button handler
        [HttpPost]
        public JsonResult SaveGameClick(string JSONGame, int GameID)
        {
            Game g = JsonConvert.DeserializeObject<Game>(JSONGame);

            bool IsNewGame = (g.ID == 0);
            
            g = ScoreHelper.ScoreGame(g);

            g.UserID = _userManager.GetUserId(User);
            g = DataHelper.SaveGame(g, _db);

            if (g.SeriesID != null && g.SeriesID != 0)
            {
                //update series
                DataHelper.UpdateSeries(Convert.ToInt32(g.SeriesID), _db);
            }
            
            return Json(new { jsonGameReturned = JsonConvert.SerializeObject(g)/*, redirect = IsNewGame*/ });
        }
        #endregion

    }
}