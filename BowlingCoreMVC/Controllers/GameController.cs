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
        [Authorize]
        public async Task<IActionResult> Index(int? SeriesPage, int? GamePage)
        {
            var user = await GetCurrentUserAsync();
            int PageSize = 12;

            // NOTE(ERIC): All Games vs NonSeries Games? DataHelper.GetNonSeriesGamesByUserID(user.Id, _db);

            IQueryable<Series> series = DataHelper.GetAllSeriesByUserIDQueryable(user.Id, _db);
            //await series.ForEachAsync(o => o.LeagueName = (_db.Leagues.Where(l => l.ID == o.LeagueID).Select(ln => ln.Name).DefaultIfEmpty("").ToString()));

            //*************
            // This one was somewhat working, but paging broke. It needs to be async??
            //List<Series> s = series.ToList();
            //s.ForEach(o => o.LeagueName = DataHelper.GetLeagueNameByID(o.LeagueID ?? 0, _db));
            //PaginatedList<Series> pagSeries = new PaginatedList<Series>(s, s.Count, 1, 3);
            //*************


            ViewData["UserSeries"] = 
                await PaginatedList<Series>.CreateAsync(series, SeriesPage ?? 1, PageSize);
            //ViewData["UserSeries"] = pagSeries;

            IQueryable <Game> games = _db.Games
                .Where(o => o.UserID == user.Id)
                .Include(o => o.Frames)
                .OrderByDescending(o => o.CreatedDate).AsNoTracking();

            


            ViewData["UserGames"] = await PaginatedList<Game>.
                CreateAsync(games, 
                GamePage ?? 1, PageSize);


            return View();
        }

        // GET Create (Create a new game, redirect to Edit page)
        //[HttpGet]
        //public ActionResult Create()
        //{
        //    Game g = Game.Create();
        //    return View("Edit", g);
        //}

        // GET Edit page (Edit a single game by ID)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Game game;
            var user = await GetCurrentUserAsync();
            if (user == null) { return RedirectToAction("Login", "Account"); }

            
            if (id == 0)
            {
                game = Game.Create();
                game.UserName = user.UserName;
                game.UserID = user.Id;
                game.User = user;

                // Save the game immediately, so the Tags on the page are not 0
                game = DataHelper.SaveGame(game, _db);
            }
            else
            {
                game = _db.Games.Include(o => o.Frames).Include(o => o.User).Where(g => g.ID == id).SingleOrDefault();
            }
            
            game.Frames = game.Frames.OrderBy(f => f.FrameNum).ToList();

            

            return View(game);
        }

        // GET details page
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
        [Authorize]
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game= await _db.Games.SingleOrDefaultAsync(m => m.ID == id);
            _db.Games.Remove(game);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        #region "editgame buttons"
#if false
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
#endif
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
                // Update series (to recalculate series score)
                DataHelper.UpdateSeries(Convert.ToInt32(g.SeriesID), _db);
            }
            
            return Json(new { jsonGameReturned = JsonConvert.SerializeObject(g)/*, redirect = IsNewGame*/ });
        }
#endregion

    }
}
