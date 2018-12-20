using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BowlingCoreMVC.Models
{
    // Testing
    public class ResultItem
    {
        public string UserID;
        public string UserName;
        public double Average;
    }

    public class Frame
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int GameID { get; set; }

        public int FrameNum { get; set; }
        public int FrameScore { get; set; }
        public int FrameTotal { get; set; }

        public int ThrowOneScore { get; set; }
        public int ThrowTwoScore { get; set; }
        public int ThrowThreeScore { get; set; }

        //These are int16s bit fields of the missed pins
        // Example: Missed 10 pin: 1000000000
        // Example: Missed washout: 1000101011 (1-2-4-6-10)
        public short ThrowOnePins { get; set; }
        public short ThrowTwoPins { get; set; }
        public short ThrowThreePins { get; set; }

        //[Editable(true)]
        //public bool PinMissed { get; set; }
    }

    public class Game
    {
        public static Game Create(int SeriesID = 0)
        {
            var g = new Game();
            g.ID = 0;
            g.CurrentFrame = 1;
            g.CurrentThrow = 1;
            g.ScoreUpToFrame = 1;
            g.CreatedDate = DateTime.Now;
            g.ModifiedDate = DateTime.Now;
            g.SeriesID = SeriesID;
            //g.UserID = ; //TODO: Set current User

            g.Frames = new List<Frame>();

            for (int i = 1; i <= 10; i++)
            {
                Frame f = new Frame();
                f.FrameNum = i;

                g.Frames.Add(f);
            }
            return (g);
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int Score { get; set; }

        [Display(Name="Created")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy (ddd) hh:mm tt}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy (ddd) hh:mm tt}")]
        public DateTime ModifiedDate { get; set; }

        public int CurrentFrame { get; set; }
        public int CurrentThrow { get; set; }

        //This is to score the game only up to this frame
        //this number should only go up, max of 10
        public int ScoreUpToFrame { get; set; }

        [ForeignKey("GameID")]
        public virtual ICollection<Frame> Frames { get; set; }


        //nvarcher(128)
        [StringLength(128)]
        public string UserID { get; set; }

        public int SeriesIndex { get; set; }

        public int? SeriesID { get; set; }

        //test
        //public virtual Series Series { get; set; }

        [NotMapped]
        public string UserName { get; set; }
    }

    public class Series
    {
        public static Series Create(int NumberOfGames, int LeagueID = 0)
        {
            Series s = new Series();
            s.CreatedDate = DateTime.Now;
            s.ModifiedDate = DateTime.Now;
            s.LeagueID = LeagueID;
            //s.UserID = ; //TODO: Set current User
            s.SeriesScore = 0;

            s.Games = new List<Game>();
            for (int i = 1; i <= NumberOfGames; i++)
            {
                Game g = Game.Create();
                g.SeriesIndex = i;
                s.Games.Add(g);
            }

            return (s);
        }


        public int ID { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        //[NotMapped]
        public int? LeagueID { get; set; }

        //public int? TeamID {get;set;}

        public int SeriesScore { get; set; }

        public virtual ICollection<Game> Games { get; set; }

        [Display(Name = "Created")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy (ddd) hh:mm tt}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy (ddd) hh:mm tt}")]
        public DateTime ModifiedDate { get; set; }


        [NotMapped]
        public List<SelectListItem> Leagues { get; set; }

        [NotMapped]
        [Range(2, 9)]
        public int NumberOfGames { get; set; }

        [NotMapped]
        [Display(Name = "League")]
        public string LeagueName { get; set; }

        [NotMapped]
        public string UserName { get; set; }
    }

    [DisplayColumn("Name")]
    public class League : IValidatableObject
    {
        public static League Create()
        {
            League l = new League();
            l.CreatedDate = DateTime.Now;
            l.ModifiedDate = DateTime.Now;
            return (l);
        }

        public int ID { get; set; }

        [Display(Name = "Select a Location")]
        public int? LocationID { get; set; }

        public string Name { get; set; }

        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = @"{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = @"{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }


        [StringLength(128)]
        public string CreatedByID { get; set; }

        [Display(Name = "Created")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy (ddd) hh:mm tt}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy (ddd) hh:mm tt}")]
        public DateTime ModifiedDate { get; set; }

        public virtual Location Location { get; set; }

        [NotMapped]
        [Display(Name = "Created By")]
        public string CreatedByUserName { get; set; }

        [NotMapped]
        public List<SelectListItem> Locations { get; set; }

        [NotMapped]
        [Display(Name = "Create a new Location?")]
        public bool NewLocation { get; set; }

        [NotMapped]
        [Display(Name = "New Location Name")]
        public string NewLocationName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
        {
            if (NewLocation && string.IsNullOrEmpty(NewLocationName))
            {
                yield return new ValidationResult("New Location Name Required when creating a new Location.");
            }

            if (!NewLocation && (LocationID == null || LocationID == 0))
            {
                yield return new ValidationResult("Select either a current location or choose to create a new one.");
            }
            
        }

    }

    public class LeagueUsers
    {
        public int ID { get; set; }
        public int LeagueID { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        //Add a flag here?
    }
    

    /*
     * League > LeagueUsers
     * LeagueUsers > User
     * User > Series
     * Series > Game
     * 
     * */


    public class Location
    {
        public int ID { get; set; }
        public string Name { get; set; }

        [StringLength(128)]
        public string CreatedByID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        [NotMapped]
        [Display(Name="Created By")]
        public string CreatedByUserName { get; set; }
    }

    // NOTE: Not implimented yet
    // Need to update Series: replace LeagueID with TeamID
    public class Team
    {
        public static Team Create()
        {
            Team t = new Team();
            t.CreatedDate = DateTime.Now;
            t.ModifiedDate = DateTime.Now;
            return (t);
        }

        public int ID { get; set; }
        public int LeagueID { get; set; }
        public string TeamName { get; set; }

        [NotMapped]
        public List<SelectListItem> Leagues {get;set;}

        public virtual League League { get; set; }

        [NotMapped]
        [Display(Name="Created By")]
        public string CreatedByUserName { get; set; }


        [StringLength(128)]
        public string CreatedByID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class TeamToUser
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public int TeamID { get; set; }
    }

}
