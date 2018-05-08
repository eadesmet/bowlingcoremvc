using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BowlingCoreMVC.Data;
using BowlingCoreMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public static void CreateSeries(Series s, ApplicationDbContext db)
        {
            db.Attach(s);
            db.Entry(s).State = EntityState.Added;
            db.SaveChanges();

            foreach (var g in s.Games)
            {
                g.SeriesID = s.ID;
            }

            db.SaveChanges();
        }

        public static void UpdateSeries(int SeriesID, ApplicationDbContext db)
        {
            var series = db.Series.Where(o => o.ID == SeriesID).Include(o => o.Games).SingleOrDefault();
            series.SeriesScore = 0;
            series.ModifiedDate = DateTime.Now;

            foreach (var g in series.Games)
            {
                series.SeriesScore += g.Score;
            }

            db.SaveChanges();
        }
        #endregion


        public static List<SelectListItem> GetCurrentLeagues(ApplicationDbContext _db)
        {
            var result = new List<SelectListItem>();
            var leagues = _db.Leagues.Where(o => o.EndDate >= DateTime.Today).ToList();
            foreach (var l in leagues)
            {
                result.Add(new SelectListItem() { Value = l.ID.ToString(), Text = l.Name });
            }

            return (result);
        }

        public static List<SelectListItem> GetAllLocations(ApplicationDbContext _db)
        {
            var result = new List<SelectListItem>();
            var locations = _db.Locations.ToList();
            foreach (var l in locations)
            {
                result.Add(new SelectListItem() { Value = l.ID.ToString(), Text = l.Name });
            }

            return (result);
        }
    }
}
