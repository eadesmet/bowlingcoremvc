using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BowlingCoreMVC.Models.GameViewModels
{
    public class GameViewModels
    {
        public class SeriesViewModel
        {
            public int? LeagueID { get; set; }

            public List<SelectListItem> Leagues { get; set; }

            [Range(0, 9)]
            public int NumberOfGames { get; set; }
        }
    }
}
