using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BowlingCoreMVC.Models;
using BowlingCoreMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace BowlingCoreMVC.ViewComponents
{
    public class EditGameViewComponent : ViewComponent
    {
        private ApplicationDbContext _db { get; set; }

        public EditGameViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int GameID)
        {
            var game = await _db.Games.Include(o => o.Frames).Where(g => g.ID == GameID).SingleOrDefaultAsync();
            if (game == null)
            {
                game = Game.Create();
            }
            else
            {
                game.Frames = game.Frames.OrderBy(f => f.FrameNum).ToList();
            }
            
            return View(game);
        }

    }
}
