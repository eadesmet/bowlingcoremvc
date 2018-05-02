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
        public static readonly ApplicationDbContext db;

        #region Games
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

                DBGame.ModifiedDate = DateTime.Now;

                db.SaveChanges();

            }
            else
            {
                //create new game
                DBGame = PageGame;
                DBGame.CreatedDate = DateTime.Now;
                DBGame.ModifiedDate = DateTime.Now;
                //Other fields??

                //NOTE: Maybe db.Games.Add(DBGame); here
                db.Attach(DBGame);
                db.Entry(DBGame).State = EntityState.Added;
                db.SaveChanges();

                
                foreach (var f in DBGame.Frames)
                {
                    f.GameID = DBGame.ID;
                }

                //TODO: Confirm State is picked up automatically on Frame update
                //db.Entry(DBGame).State = EntityState.Modified;
                db.SaveChanges();
            }
            
            

            return (DBGame);
        }
        #endregion
        #region Series/leagues
        //public static Dictionary<string, int> GetCurrentLeagueDict()
        //{
        //    Dictionary<string, int> LeagueDict = new Dictionary<string, int>();

        //    LeagueDict = db.Leagues.Where(o => o.EndDate <= DateTime.Today).ToDictionary(o => o.Name, o => o.ID);

        //    return (LeagueDict);
        //}
        #endregion
    }
}
