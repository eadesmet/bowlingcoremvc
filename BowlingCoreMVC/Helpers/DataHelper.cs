using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BowlingCoreMVC.Data;
using BowlingCoreMVC.Models;

using Microsoft.EntityFrameworkCore;

namespace BowlingCoreMVC.Helpers
{
    public static class DataHelper
    {

        //Ideally, I wouldn't have to do this call at all
        //what's the point
        //if we have a game from a page, why would it need MORE fields than the page has already?
        //public static Game UpdateGame(Game PageGame, ApplicationDbContext db)
        //{
        //    //TODO: Set fields I want updated from PageGame to DBGame
        //    var DBGame = db.Games.Include(o => o.Frames).Where(o => o.ID == PageGame.ID).SingleOrDefault();
        //    DBGame.Frames = DBGame.Frames.OrderBy(f => f.FrameNum).ToList();

        //    DBGame.ModifiedDate = DateTime.Now;
        //    DBGame.Score = PageGame.Score;
        //    DBGame.CurrentFrame = PageGame.CurrentFrame;
        //    DBGame.CurrentThrow = PageGame.CurrentThrow;
        //    DBGame.ScoreUpToFrame = PageGame.ScoreUpToFrame;

        //    var zip_frames = DBGame.Frames.Zip(PageGame.Frames, (e, n) => new { DB_f = e, Page_f = n });

        //    foreach (var f in zip_frames)
        //    {
        //        f.DB_f.FrameScore = f.Page_f.FrameScore;
        //        f.DB_f.FrameTotal = f.Page_f.FrameTotal;

        //        f.DB_f.ThrowOnePins = f.Page_f.ThrowOnePins;
        //        f.DB_f.ThrowTwoPins = f.Page_f.ThrowTwoPins;
        //        f.DB_f.ThrowThreePins = f.Page_f.ThrowThreePins;

        //        f.DB_f.ThrowOneScore = f.Page_f.ThrowOneScore;
        //        f.DB_f.ThrowTwoScore = f.Page_f.ThrowTwoScore;
        //        f.DB_f.ThrowThreeScore = f.Page_f.ThrowThreeScore;

        //    }

        //    return (DBGame);
        //}

        public static Game SaveGame(Game PageGame, ApplicationDbContext db)
        {
            Game DBGame;
            if (PageGame.ID != 0)
            {
                //Update existing game
                DBGame = db.Games.Include(o => o.Frames).Where(o => o.ID == PageGame.ID).SingleOrDefault();
                //DBGame.Frames = DBGame.Frames.OrderBy(f => f.FrameNum).ToList();

                //update DBgame with updated fields from PageGame
                DBGame.CurrentFrame = PageGame.CurrentFrame;
                DBGame.CurrentThrow = PageGame.CurrentThrow;
                DBGame.Frames = PageGame.Frames;
                DBGame.Score = PageGame.Score;
                DBGame.ScoreUpToFrame = PageGame.ScoreUpToFrame;
                
                db.SaveChanges();

            }
            else
            {
                //create new game
                DBGame = PageGame;
                DBGame.CreatedDate = DateTime.Now;
                DBGame.ModifiedDate = DateTime.Now;
                //Other fields??

                db.Attach(DBGame);
                db.Entry(DBGame).State = EntityState.Added;
                db.SaveChanges();
            }
            
            

            return (DBGame);
        }
    }
}
