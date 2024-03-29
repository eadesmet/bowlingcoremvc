﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using BowlingCoreMVC.Models;
using BowlingCoreMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace BowlingCoreMVC.ViewComponents
{
    public class GameDetailsViewComponent : ViewComponent
    {
        private ApplicationDbContext _db { get; set; }

        public GameDetailsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int GameID)
        {
            var game = await _db.Games.Include(o => o.Frames).Where(g => g.ID == GameID).SingleOrDefaultAsync();
            //NOTE: Why am I creating a game if one doesn't exist? I should just redirect
            if (game == null)
            {
                game = Game.Create();
            }
            else
            {
                game.Frames = game.Frames.OrderBy(f => f.FrameNum).ToList();
            }

            return View(game); //'default' inside our viewcomponent folder Views/Shared/components
        }
    }
}

