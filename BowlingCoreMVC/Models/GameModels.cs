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
        public static Game Create(int? SeriesID = null)
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


        //nvarcher(450)
        [StringLength(450)]
        public string UserID { get; set; }

        public int SeriesIndex { get; set; }

        public int? SeriesID { get; set; }

        //test
        //public virtual Series Series { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        [NotMapped]
        public string UserName { get; set; }
    }

    public class Series
    {
        public static Series Create(int NumberOfGames, int? LeagueID = null, int? TeamID = null)
        {
            Series s = new Series();
            s.CreatedDate = DateTime.Now;
            s.ModifiedDate = DateTime.Now;
            s.LeagueID = LeagueID;
            //s.UserID = ; //TODO: Set current User
            s.SeriesScore = 0;
            s.TeamID = TeamID;

            s.Games = new List<Game>();
            for (int i = 1; i <= NumberOfGames; i++)
            {
                Game g = Game.Create(s.ID);
                g.SeriesIndex = i;
                s.Games.Add(g);
            }

            return (s);
        }


        public int ID { get; set; }

        [StringLength(450)]
        public string UserID { get; set; }

        public int? LeagueID { get; set; }

        public int? TeamID {get;set;}

        public int SeriesScore { get; set; }

        public virtual ICollection<Game> Games { get; set; }

        [Display(Name = "Created")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy (ddd) hh:mm tt}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy (ddd) hh:mm tt}")]
        public DateTime ModifiedDate { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("LeagueID")]
        public virtual League League { get; set; }

        [ForeignKey("TeamID")]
        public virtual Team Team { get; set; }

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

    // TODO(ERIC): Will need some queries behind this to get when a league occurs
    public enum LeagueOccurance
    {
        EveryWeek = 1,
        EveryOtherWeek = 2,
        EveryMonth = 4
    }

    [DisplayColumn("Name")]
    public class League : IValidatableObject
    {
        public static League Create()
        {
            League l = new League();
            l.CreatedDate = DateTime.Now;
            l.ModifiedDate = DateTime.Now;
            l.Occurance = LeagueOccurance.EveryWeek;
            l.DefaultNumOfGames = 3;
            return (l);
        }

        public int ID { get; set; }

        [Display(Name = "Select a Location")]
        public int? LocationID { get; set; }

        [Required(ErrorMessage = "Please provide a Name for the League")]
        public string Name { get; set; }

        [Display(Name = "League Message")]
        public string Message { get; set; }

        [Display(Name = "Occurance (How often the league bowls)")]
        public LeagueOccurance Occurance { get; set; }

        [Range(2, 9)]
        [Display(Name = "How many games are played each day?")]
        public int DefaultNumOfGames { get; set; }

        [Display(Name="Leagues first day (Start Date)")]
        [Required(ErrorMessage = "Please provide a Start Date for the League")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Leagues last day (End Date)")]
        [Required(ErrorMessage = "Please provide an End Date for the League")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name="Which day of the week is the league bowled on?")]
        [Required(ErrorMessage = "Please select a Day")]
        [Range(0, 6, ErrorMessage = "Please enter a the Day of the week this league is bowled on")]
        public DayOfWeek LeagueDay { get; set; }

        [NotMapped]
        public List<SelectListItem> Days { get; set; }

        [StringLength(450)]
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
        public List<SelectListItem> Occurances { get; set; }

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

    public class Location
    {
        public int ID { get; set; }
        public string Name { get; set; }

        [StringLength(450)]
        public string CreatedByID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        [NotMapped]
        [Display(Name="Created By")]
        public string CreatedByUserName { get; set; }
    }

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

        [ForeignKey("LeagueID")]
        public virtual League League { get; set; }

        [NotMapped]
        [Display(Name="Created By")]
        public string CreatedByUserName { get; set; }

        [NotMapped]
        public List<string> TeamMembers { get; set; }

        public virtual ICollection<UserLeagueTeam> UserLeagueTeams { get; set; }

        [StringLength(450)]
        public string CreatedByID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class UserLeagueTeam
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(450)]
        public string UserID { get; set; }

        [Display(Name = "League")]
        [Required(ErrorMessage = "Please select a League")]
        public int LeagueID { get; set; }

        [Display(Name = "Team")]
        public int TeamID { get; set; }

        // TODO(Eric): Consider adding the Average here for less DB queries

        [NotMapped]
        public List<SelectListItem> Leagues {get;set;}

        [NotMapped]
        public List<SelectListItem> Teams {get;set;}

        // If Today is before the League End date
        public bool IsActive { get; set; }

        // If the user is an Owner/Admin of this League/Team
        // NOTE: Might want to split this out between League and Team
        //  CreatedByID should default to IsAdmin!
        public bool IsAdmin { get; set; }
    }

}
