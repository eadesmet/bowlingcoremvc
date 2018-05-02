using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingCoreMVC.Models
{
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
        public static Game Create()
        {
            var g = new Game();
            g.ID = 0;
            g.CurrentFrame = 1;
            g.CurrentThrow = 1;
            g.ScoreUpToFrame = 1;
            g.CreatedDate = DateTime.Now;
            g.ModifiedDate = DateTime.Now;

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
        public DateTime CreatedDate { get; set; }
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

    }

    public class Series
    {
        public int ID { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        [Display(Name ="Name")]
        public int? LeagueID { get; set; }

        public int SeriesScore { get; set; }

        public virtual ICollection<Game> Games { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

    }

    public class League
    {
        public int ID { get; set; }

        public string Name { get; set; }

        [StringLength(128)]
        public string CreatedByID { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

    }

    public class LeagueUsers
    {
        public int ID { get; set; }
        public int LeagueID { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }
    }
    

    /*
     * League > LeagueUsers
     * LeagueUsers > User
     * User > Series
     * Series > Game
     * 
     * */
}
