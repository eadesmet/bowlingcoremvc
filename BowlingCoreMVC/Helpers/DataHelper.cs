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
    // Trying this out
    public struct DBOperationResult<T>
    {
        public bool IsError;
        public string Message;
        public T Item;
    }

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
        public static Series InsertSeries(Series s, ApplicationDbContext db, string UserID)
        {
            s.UserID = UserID;
            db.Attach(s);
            db.Entry(s).State = EntityState.Added;
            db.SaveChanges();

            //string UserName = GetUserNameFromID(UserID, db);
            
            foreach (var g in s.Games)
            {
                g.SeriesID = s.ID;
                g.UserID = UserID;
                g.UserName = s.User.UserName;// UserName;
            }
            
            db.SaveChanges();

            //s.UserName = UserName;
            
            return (s);
        }
        
        public static DBOperationResult<Series>
        CreateAndInsertSeries(ApplicationDbContext db, 
            string UserID, int? LeagueID = null, int? TeamID = null)
        {
            DBOperationResult<Series> Result = new DBOperationResult<Series>();

            int NumOfGames = 3;
            if (LeagueID != null)
            {
                League l = db.Leagues.Where(o => o.ID == LeagueID).SingleOrDefault();
                if (l.DefaultNumOfGames > 0)
                    NumOfGames = l.DefaultNumOfGames;

                if (!ValidateCanCreateSeries(db, UserID, l))
                {
                    Result.IsError = true;
                    Result.Message = "Cannot create a Series - Either you already have one for this week, or it's the end of the league";
                    Result.Item = null;
                    return (Result);
                }
            }

            Series s = Series.Create(NumOfGames, LeagueID, TeamID);
            s.UserID = UserID;
            s.UserName = GetUserNameFromID(UserID, db);

            s.TeamID = TeamID;

            db.Attach(s);
            db.Entry(s).State = EntityState.Added;
            db.SaveChanges();

            foreach (var g in s.Games)
            {
                g.SeriesID = s.ID;
                g.UserID = UserID;
                g.UserName = s.User.UserName;// UserName;
            }
            
            db.SaveChanges();

            Result.Item = s;
            return (Result);
        }

        private static bool
        ValidateCanCreateSeries(ApplicationDbContext db, 
            string UserID, League l)
        {
            // Can't create a Series if its not on the same day
            if (DateTime.Today.DayOfWeek != l.LeagueDay)
                return (false);

            DateTime NextLeagueNight = GetNextLeagueNight(l);
            // Can't create a Series if its after the end of the league
            if (NextLeagueNight > l.EndDate)
                return (false);

            // Can't create a Series if they already have for for that day
            if (db.Series.Where(o => o.UserID == UserID && o.CreatedDate.Date == DateTime.Today && o.LeagueID == l.ID).Any())
                return (false);



            return (true);
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

        public static List<SelectListItem> GetAllDays()
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            foreach (var day in Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList())
            {
                Result.Add(new SelectListItem() { Value = ((int)day).ToString(), Text = day.ToString() });
            }
            return (Result);
        }

        public static List<SelectListItem> GetAllOccurances()
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            foreach (var occ in Enum.GetValues(typeof(LeagueOccurance)).Cast<LeagueOccurance>().ToList())
            {
                Result.Add(new SelectListItem() { Value = ((int)occ).ToString(), Text = occ.ToString() });
            }
            return (Result);
        }


        public static List<League> UserLeagues(string UserID, ApplicationDbContext _db)
        {
            //user is in a league if they have a series in that league
            var result = new List<League>();
            var UserLeagueTeams = _db.UserLeagueTeams.Where(o => o.UserID == UserID).ToList();
            foreach (var ult in UserLeagueTeams)
            {
                //Do not add duplicates (legacy - was from series search. only needed if multiple teams for same league)
                if (result.Find(o => o.ID == ult.LeagueID) == null)
                {
                    result.Add(_db.Leagues.Where(o => o.ID == ult.LeagueID).SingleOrDefault());
                }
            
            }
            
            return (result);
        }

        public static int? GetTeamIfExists(int LeagueID, string UserID, ApplicationDbContext _db)
        {
            int? Result = null;

            // TODO(ERIC): Handle multiple teams? Only allow one??
            var q = _db.UserLeagueTeams.Where(o => o.UserID == UserID && o.LeagueID == LeagueID).FirstOrDefault();
            if (q != null) { Result = q.TeamID; }
            return (Result);
        }

        public static void InsertUserLeagueTeam(UserLeagueTeam ult, ApplicationDbContext _db)
        {
            _db.UserLeagueTeams.Add(ult);
            _db.SaveChanges();
            return;
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
            // TODO(ERIC): Clean this up. Minimize DB calls, it's crazy now.
            List<Series> Result = (_db.Series.Include(o => o.Games).Where(o => o.UserID == UserID).OrderByDescending(o => o.CreatedDate).AsNoTracking().ToList());
            //string UserName = GetUserNameFromID(UserID, _db);
            foreach (var s in Result)
            {
                //s.LeagueName = GetLeagueNameByID(s.LeagueID ?? 0, _db);
                //s.UserName = UserName;
            }
            return (Result);
        }

        public class SeriesQueryResult
        {
            public int ID;
            public int SeriesScore;
            public string LeagueName;
            public DateTime CreatedDate;
            public List<Game> Games;
        }

        public static IQueryable<Series> GetAllSeriesByUserIDQueryable(string UserID, ApplicationDbContext _db)
        {
            IQueryable<Series> Result = (IQueryable<Series>)_db.Series
                .Include(o => o.Games).ThenInclude(x => x.Frames)
                .Include(o => o.League)
                .Where(o => o.UserID == UserID)
                .OrderByDescending(o => o.CreatedDate)
                .AsNoTracking();

            return (Result);
        }

        //public static string GetLeagueNameByID(int LeagueID, ApplicationDbContext _db)
        //{
        //    League Result = _db.Leagues.Where(o => o.ID == LeagueID).SingleOrDefault();
        //    return (Result != null) ? Result.Name : "";
        //}

        public static DateTime GetNextLeagueNight(DateTime StartDate)
        {
            // Not confirming if it's before the EndDate here.
            DayOfWeek LeagueNight = StartDate.DayOfWeek;
            int DaysUntil = ((int)LeagueNight - (int)DateTime.Today.DayOfWeek + 7) % 7;

            return (DateTime.Today.AddDays(DaysUntil));
        }

        public static DateTime GetNextLeagueNight(League l)
        {
            int DaysUntil = -1;
            DayOfWeek LeagueDayOfWeek = l.LeagueDay;
            if (l.Occurance == LeagueOccurance.EveryWeek)
            {
                DaysUntil = ((int)LeagueDayOfWeek - (int)DateTime.Today.DayOfWeek + 7) % 7;
            }
            else if (l.Occurance == LeagueOccurance.EveryOtherWeek)
            {
                DaysUntil = ((int)LeagueDayOfWeek - (int)DateTime.Today.DayOfWeek + 14) % 14;
            }
            else if (l.Occurance == LeagueOccurance.EveryMonth)
            {
                DaysUntil = ((int)LeagueDayOfWeek - (int)DateTime.Today.DayOfWeek + 28) % 28;
            }

            // Scenerio 1:
            //      It is Wednesday, League night is Tuesday
            //      I want to get the Next Tuesday


            
            DateTime Result = DateTime.Today.AddDays(DaysUntil);

            // Confirm that it is before the end date (maybe not needed?)
            if (Result > l.EndDate)
            {
                 // What would we even return here? Today? null? Last league night?
                Result = DateTime.Today.AddYears(-50);
            }

            return (Result);
        }

        public static string GetUserNameFromID(string UserID, ApplicationDbContext _db)
        {
            return (_db.Users.Where(o => o.Id == UserID).SingleOrDefault().UserName);
        }

        #region Teams
        public static List<TeamLastWeekData> GetTeamLastWeekData(int LeagueID, ApplicationDbContext _db)
        {
            List<TeamLastWeekData> Result = new List<TeamLastWeekData>();

            var LeagueTeams = _db.Teams.Where(o => o.LeagueID == LeagueID).Include(o => o.UserLeagueTeams).ToList();

            foreach (var Team in LeagueTeams)
            {
                TeamLastWeekData TeamData = new TeamLastWeekData();
                TeamData.TeamName = Team.TeamName;
                // Get all users in the team

                // Can I get this result from Team.TeamMembers instead of another db call?
                //var UserLeagueTeam = _db.UserLeagueTeams.Where(o => o.TeamID == Team.ID).ToList();
                //foreach (var ult in UserLeagueTeam)
                foreach (var ult in Team.UserLeagueTeams)
                {
                    // UserLeagueTeam - Has UserIDs and the Team, and the League
                    // Need to get their Average, their Last Series, Games Count and Games Total
                    
                    // TODO(ERIC): FIRST THING ACTUALLY!!!
                    // REDO ALL OF THESE QUERIES AND OBJECTS, HOLY CRAP
                    // CAN MINIMIZE QUERIES SUBSTANTIALLY AND GATHER THEM MUCH MORE NICELY
                    // ALSO, REMEMBER NULL CHECKS HERE AND IN THE VIEW

                    // NOTE(ERIC) 3-12-19: I'm not sure.. This all seems reasonable..
                    //
                    // I have one database call for all the Teams in this League
                    // Then in GetUserTEamWeekData I have one database call to get all of that users games.
                    //    well, so in reality, for every team......
                    //    let's say a league has 5 teams, each with 5 members
                    //    I'll then have 1 + (5 * 5) database calls (26)
                    //
                    // Then would creating one big query (teams.include(Series).include(games)) etc. be faster?
                    // 


                    //Series UserSeries = GetLastUserTeamSeries(ult.UserID, ult.TeamID, ult.LeagueID, _db);
                    UserTeamWeekData UserWeekData = GetUserTeamWeekData(ult.UserID, ult.TeamID.Value, ult.LeagueID, _db);

                    string UserName = GetUserNameFromID(ult.UserID, _db);
                    TeamData.UserNames.Add(UserName);
                    TeamData.Averages.Add(UserWeekData.Average);
                    TeamData.Series.Add(UserWeekData.Series);
                    TeamData.TotalGames.Add(UserWeekData.TotalGames);
                    TeamData.TotalPins.Add(UserWeekData.TotalPins);

                    Result.Add(TeamData);
                }
            }


            return (Result);
        }

        public struct UserTeamWeekData
        {
            public double Average;
            public int TotalPins;
            public int TotalGames;
            public Series Series;
        }

        public static UserTeamWeekData GetUserTeamWeekData(string UserID, int TeamID, int LeagueID, ApplicationDbContext _db)
        {
            //
            // NOTE(ERic): I wonder how I can narrow this down..
            // So basically I need only a weeks worth of data
            // Can I narrow it down by date somehow? or do an order by date, take 1?
            // I also need to take nulls into account
            //


            // All series of League + Team + User
            // so THIS is why i'm doing this..
            // To calculate their average and total pins for the Team+League, I need to get All their series from it
            var UserTeamSeries = _db.Series.Where(o => o.UserID == UserID && o.LeagueID == LeagueID && o.TeamID == TeamID).Include(o => o.Games);
            

            List<Game> Games = new List<Game>();
            foreach (var s in UserTeamSeries)
            {
                foreach(var g in s.Games)
                {
                    Games.Add(g);
                }
            }
            //var Result = UserTeamSeries.Select(o => o.Games.Average(g => g.Score));
            //return (Result.SingleOrDefault());
            UserTeamWeekData Result = new UserTeamWeekData();
            if (UserTeamSeries == null || !UserTeamSeries.Any()) { return Result; }
            if (Games != null)
            {
                Result.TotalGames = Games.Count;
                Result.Average = Games.Average(g => g.Score);
                Result.TotalPins = Games.Sum(g => g.Score);
            }

            // TODO(ERIC): ENsure this is the last series they bowled.
            Result.Series = UserTeamSeries.OrderBy(o => o.CreatedDate).Take(1).SingleOrDefault();

            return (Result);
        }

        public static Series GetLastUserTeamSeries(string UserID, int TeamID, int LeagueID, ApplicationDbContext _db)
        {
            // TODO(ERIC): Get the Range of dates, not just now - 7
            return _db.Series.Where(o => o.UserID == UserID && o.LeagueID == LeagueID && o.TeamID == TeamID && o.CreatedDate >= DateTime.Now.AddDays(-7)).Include(o => o.Games).SingleOrDefault();
        }

        
        #endregion
    }
}
