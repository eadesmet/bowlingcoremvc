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

    public class ListSingleValue
    {
        public ListSingleValue()
        {
            this.Keys = new List<string>();
            this.Values = new List<int>();
        }

        public string Title { get; set; }
        public string SubTitle { get; set; }

        public List<string> Keys { get; set; }
        public List<int> Values { get; set; }
    }

    
    // NOTE(ERIC): I'd like to redo this data structure to be better and more cool.
    //
    // I need something like this in the end:
    //
    // User
    //    Users Leagues
    //        Users Team In That League
    //            All Users in this Team's Last Week Data
    // This should actually, probably, be a stored proc
    // that returns a custom table that I then know how to query myself
    // so I could narrow it down by league/team afterwards.
    public class TeamLastWeekData
    {
        public TeamLastWeekData()
        {
            this.UserNames = new List<string>();
            this.Averages = new List<double>();
            this.TotalPins = new List<int>();
            this.TotalGames = new List<int>();
            this.Series = new List<Series>();
        }
        public string TeamName { get; set; }
        public string SubTitle { get; set; }

        public List<string> UserNames { get; set; }
        public List<double> Averages { get; set; }
        public List<int> TotalPins { get; set; }
        public List<int> TotalGames { get; set; }
        public List<Series> Series { get; set; }
        // From the Series I can get Series total.
    }

}
