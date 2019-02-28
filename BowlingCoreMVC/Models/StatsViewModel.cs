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
        public string Title { get; set; }
        public string SubTitle { get; set; }

        public List<string> Keys { get; set; }
        public List<int> Values { get; set; }
    }

    public class TeamLastWeekData
    {
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
