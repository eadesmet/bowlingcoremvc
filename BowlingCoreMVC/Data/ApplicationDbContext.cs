﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BowlingCoreMVC.Models;

namespace BowlingCoreMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            //builder.Entity<Game>()
            //    .HasOne(g => g.Series)
            //    .WithMany(s => s.Games)
            //    .OnDelete(DeleteBehavior.Cascade);

            // Convert the LeagueOccurance enum to a String in the database
            // When I get it in code, it should still be an enum
            //builder
            //    .Entity<League>()
            //    .Property(e => e.Occurance)
            //    .HasConversion(
            //        v => v.ToString(),
            //        v => (LeagueOccurance)Enum.Parse(typeof(LeagueOccurance), v));
            builder
                .Entity<League>()
                .Property(e => e.DefaultNumOfGames)
                .HasDefaultValue(3);

            builder
                .Entity<League>()
                .Property(e => e.Occurance)
                .HasDefaultValue(LeagueOccurance.EveryWeek);


        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Frame> Frames { get; set; }

        public DbSet<Series> Series { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Location> Locations { get; set; }

        public DbSet<Team> Teams { get; set; }
        public DbSet<UserLeagueTeam> UserLeagueTeams { get; set; }

        
        //public DbSet<TeamToUser> TeamToUsers {get;set;}

    }

    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Apply migrations if any are available
            // NOTE: This call will fail if called after EnsureCreated, because microsoft:
            // https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/#apply-migrations-at-runtime
            //context.Database.Migrate();

            if (context.Games.Any())
                return;

            //var user = new ApplicationUser { UserName = "Eric", Email = "test@email.com" };
            //var result = await _userManager.CreateAsync(user, "Eric;12345");

            //var Series = new List<Series>
            //{
            //    new Series{CreatedDate=DateTime.Parse("2018-04-23"), ModifiedDate=DateTime.Parse("2018-04-23"),},
            //};

            //Series.ForEach(s => context.Series.Add(s));
            //context.SaveChanges();

            //var Games = new List<Game>
            //{
            //    new Game{Score=300, CreatedDate=DateTime.Parse("2018-02-22"), ModifiedDate=DateTime.Parse("2018-03-02"), CurrentFrame=1, CurrentThrow=1, SeriesIndex=1, SeriesID=1},
            //    new Game{Score=155, CreatedDate=DateTime.Parse("2018-03-09"), ModifiedDate=DateTime.Parse("2018-03-09"), CurrentFrame=1, CurrentThrow=1, SeriesIndex=1, SeriesID=1},
            //};

            //Games.ForEach(s => context.Games.Add(s));
            //context.SaveChanges();

            //var frames = new List<Frame>
            //{
            //    new Frame{GameID = 1, FrameNum = 1, ThrowOneScore = 10, FrameScore = 30},
            //    new Frame{GameID = 1, FrameNum = 2, ThrowOneScore = 10, FrameScore = 30},
            //    new Frame{GameID = 1, FrameNum = 3, ThrowOneScore = 10, FrameScore = 30},
            //    new Frame{GameID = 1, FrameNum = 4, ThrowOneScore = 10, FrameScore = 30},
            //    new Frame{GameID = 1, FrameNum = 5, ThrowOneScore = 10, FrameScore = 30},
            //    new Frame{GameID = 1, FrameNum = 6, ThrowOneScore = 10, FrameScore = 30},
            //    new Frame{GameID = 1, FrameNum = 7, ThrowOneScore = 10, FrameScore = 30},
            //    new Frame{GameID = 1, FrameNum = 8, ThrowOneScore = 10, FrameScore = 30},
            //    new Frame{GameID = 1, FrameNum = 9, ThrowOneScore = 10, FrameScore = 30},
            //    new Frame{GameID = 1, FrameNum = 10, ThrowOneScore = 10, ThrowTwoScore = 10, ThrowThreeScore = 10, FrameScore = 30},

            //    new Frame{GameID = 2, FrameNum = 1, ThrowOneScore = 5, ThrowTwoScore = 5, FrameScore = 15, ThrowOnePins = 0b0000_0000_0001_1111, ThrowTwoPins = 0b0000_0000_0000_0000},
            //    new Frame{GameID = 2, FrameNum = 2, ThrowOneScore = 5, ThrowTwoScore = 5, FrameScore = 15, ThrowOnePins = 0b0000_0000_0001_1111, ThrowTwoPins = 0b0000_0000_0000_0000},
            //    new Frame{GameID = 2, FrameNum = 3, ThrowOneScore = 5, ThrowTwoScore = 5, FrameScore = 15, ThrowOnePins = 0b0000_0000_0001_1111, ThrowTwoPins = 0b0000_0000_0000_0000},
            //    new Frame{GameID = 2, FrameNum = 4, ThrowOneScore = 5, ThrowTwoScore = 5, FrameScore = 15, ThrowOnePins = 0b0000_0000_0001_1111, ThrowTwoPins = 0b0000_0000_0000_0000},
            //    new Frame{GameID = 2, FrameNum = 5, ThrowOneScore = 5, ThrowTwoScore = 5, FrameScore = 15, ThrowOnePins = 0b0000_0000_0001_1111, ThrowTwoPins = 0b0000_0000_0000_0000},
            //    new Frame{GameID = 2, FrameNum = 6, ThrowOneScore = 5, ThrowTwoScore = 5, FrameScore = 15, ThrowOnePins = 0b0000_0000_0001_1111, ThrowTwoPins = 0b0000_0000_0000_0000},
            //    new Frame{GameID = 2, FrameNum = 7, ThrowOneScore = 5, ThrowTwoScore = 5, FrameScore = 15, ThrowOnePins = 0b0000_0000_0001_1111, ThrowTwoPins = 0b0000_0000_0000_0000},
            //    new Frame{GameID = 2, FrameNum = 8, ThrowOneScore = 5, ThrowTwoScore = 5, FrameScore = 15, ThrowOnePins = 0b0000_0000_0001_1111, ThrowTwoPins = 0b0000_0000_0000_0000},
            //    new Frame{GameID = 2, FrameNum = 9, ThrowOneScore = 5, ThrowTwoScore = 5, FrameScore = 15, ThrowOnePins = 0b0000_0000_0001_1111, ThrowTwoPins = 0b0000_0000_0000_0000},
            //    new Frame{GameID = 2, FrameNum = 10, ThrowOneScore = 5, ThrowTwoScore = 5, ThrowThreeScore = 5, FrameScore = 20, ThrowOnePins = 0b0000_0000_0001_1111, ThrowTwoPins = 0b0000_0000_0001_1111, ThrowThreePins = 0b0000_0000_0001_1111},
            //};

            //frames.ForEach(s => context.Frames.Add(s));
            //context.SaveChanges();
        }
    }
}
