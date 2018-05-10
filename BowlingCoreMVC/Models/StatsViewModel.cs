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

}
