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
                DBGame.UserID = PageGame.UserID;
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
        public static Series CreateSeries(Series s, ApplicationDbContext db, string UserID)
        {
            s.UserID = UserID;
            db.Attach(s);
            db.Entry(s).State = EntityState.Added;
            db.SaveChanges();
            
            foreach (var g in s.Games)
            {
                g.SeriesID = s.ID;
                g.UserID = UserID;
            }
            
            db.SaveChanges();
            
            return (s);
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
        
        public static List<League> UserLeagues(string UserID, ApplicationDbContext _db)
        {
            //user is in a league if they have a series in that league
            var result = new List<League>();
            var series = _db.Series.Where(o => o.UserID == UserID).ToList();
            foreach (var s in series)
            {
                if (s.LeagueID != null && s.LeagueID != 0)
                {
                    //Do not add duplicates
                    if (result.Find(o => o.ID == s.LeagueID) == null)
                    {
                        result.Add(_db.Leagues.Where(o => o.ID == s.LeagueID).SingleOrDefault());
                    }
                }
            }
            
            return (result);
        }
        
        public static double UsersLeagueAverage(string UserID, int LeagueID, ApplicationDbContext _db)
        {
            double result = 0.0;
            int total = 0;
            int count = 0;
            var LeaguesSeries = _db.Series.Include(o => o.Games).Where(o => o.LeagueID == LeagueID && o.UserID == UserID).ToList();
            foreach (var s in LeaguesSeries)
            {
                foreach (var g in s.Games)
                {
                    total += g.Score;
                    count++;
                }
            }
            
            result = total / count;
            
            return (result);
        }
        
        public static List<Game> GetAllGamesByUserID(string UserID, ApplicationDbContext _db)
        {
            return (_db.Games.Include(o => o.Frames).Where(o => o.UserID == UserID).OrderByDescending(o => o.CreatedDate).ToList());
        }
        
        public static List<Game> GetNonSeriesGamesByUserID(string UserID, ApplicationDbContext _db)
        {
            return (_db.Games.Include(o => o.Frames).Where(o => o.UserID == UserID).Where(o => o.SeriesID == 0 || o.SeriesID == null).OrderByDescending(o => o.CreatedDate).ToList());
        }
        
        public static List<Series> GetAllSeriesByUserID(string UserID, ApplicationDbContext _db)
        {
            List<Series> Result = (_db.Series.Include(o => o.Games).Where(o => o.UserID == UserID).OrderByDescending(o => o.CreatedDate).ToList());
            foreach(var s in Result)
            {
                s.LeagueName = GetLeagueNameByID(s.LeagueID ?? 0, _db);
            }
            return (Result);
        }
        
        public static string GetLeagueNameByID(int LeagueID, ApplicationDbContext _db)
        {
            League Result = _db.Leagues.Where(o => o.ID == LeagueID).SingleOrDefault();
            return (Result != null) ? Result.Name : "";
        }

        public static DateTime GetNextLeagueNight(DateTime StartDate)
        {
            // Not confirming if it's before the EndDate here.
            DayOfWeek LeagueNight = StartDate.DayOfWeek;
            int DaysUntil = ((int)LeagueNight - (int)DateTime.Today.DayOfWeek + 7) % 7;

            return (DateTime.Today.AddDays(DaysUntil));
        }

        public static string GetUserNameFromID(string UserID, ApplicationDbContext _db)
        {
            return (_db.Users.Where(o => o.Id == UserID).SingleOrDefault().UserName);
        }
    }
}
