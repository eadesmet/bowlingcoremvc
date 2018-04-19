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

        [Range(0, 10)]
        public int ThrowOneScore { get; set; }
        public int ThrowTwoScore { get; set; }
        public int ThrowThreeScore { get; set; }

        //These are int16s bit fields of the missed pins
        // Example: Missed 10 pin: 1000000000
        // Example: Missed washout: 1000101011 (1-2-4-6-10)
        public int ThrowOnePins { get; set; }
        public int ThrowTwoPins { get; set; }
        public int ThrowThreePins { get; set; }

        //I guess this is only for the view?
        [Editable(true)]
        public bool PinMissed { get; set; }

        //public bool TwoPinMissed   { get; set; }
        //public bool ThreePinMissed { get; set; }
        //public bool FourPinMissed  { get; set; }
        //public bool FivePinMissed  { get; set; }
        //public bool SixPinMissed   { get; set; }
        //public bool SevenPinMissed { get; set; }
        //public bool EightPinMissed { get; set; }
        //public bool NinePinMissed  { get; set; }
        //public bool TenPinMissed   { get; set; }
    }

    public class Game
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int Score { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public int CurrentFrame { get; set; }
        public int CurrentThrow { get; set; }

        //This is to score the game only up to this frame
        //this number should only go up, max of 10
        //public int ScoreUpToFrame { get; set; }

        public virtual ICollection<Frame> Frames { get; set; }


        //nvarcher(128)
        [StringLength(128)]
        public string UserID { get; set; }

        //public int SeriesIndex { get; set; }

    }

    /* Do this AFTER we publish v1
    public class Series
    {
        public int ID { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }
        public int LeagueID { get; set; }

        public int SeriesScore { get; set; }

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
    */

    /*
     * League > LeagueUsers
     * LeagueUsers > User
     * User > Series
     * Series > Game
     * 
     * */
}
