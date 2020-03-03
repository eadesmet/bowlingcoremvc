using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace BowlingCoreMVC.Models
{
    public class StatsViewModel
    {
        public string StatTitle { get; set; }

        public double Total { get; set; }
        public double Conversions { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double Percentage { get { return (this.Conversions / this.Total); } }

    }

    // To be used with "_CardListSingleValue"
    public class ListSingleValue
    {
        public ListSingleValue()
        {
            this.Keys = new List<string>();
            this.Values = new List<int>();
            this.Links = new List<string>();
        }

        public string Title { get; set; }
        public string SubTitle { get; set; }

        public List<string> Keys { get; set; }
        public List<int> Values { get; set; }
        public List<string> Links { get; set; }
    }

    // To be used with "_CardListMultipleValue"
    public class ListMultipleValue
    {
        public ListMultipleValue()
        {
            this.ColKeys = new List<List<string>>();
            this.Values = new List<int>();
            this.Links = new List<string>();
        }

        public string Title { get; set; }
        public string SubTitle { get; set; }

        public List<List<string>> ColKeys { get; set; }
        public List<int> Values { get; set; }
        public List<string> Links { get; set; }
    }

    public class TeamMember
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
    }


    public struct UserTeamWeekData
    {
        public string Username;
        public double Average;
        public int TotalPins;
        public int TotalGames;
        public Series Series;
    }

    public class TeamLastWeekData
    {
        public TeamLastWeekData()
        {
            this.UserData = new List<UserTeamWeekData>();
        }
        public string TeamName { get; set; }
        public string SubTitle { get; set; }

        public List<UserTeamWeekData> UserData { get; set; }
        
        public bool IsCurrentUserOnTeam { get; set; }
        public int TeamID { get; set; }
    }

}
