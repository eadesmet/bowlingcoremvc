﻿using System;
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
    }
}
