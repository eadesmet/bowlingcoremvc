using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BowlingCoreMVC.Data;
using BowlingCoreMVC.Models;

namespace BowlingCoreMVC.Helpers
{
    public static class DataHelper
    {
        public static Game UpdateGame(Game PageGame, Game DBGame)
        {
            //TODO: Set fields I want updated from PageGame to DBGame
            DBGame.ModifiedDate = DateTime.Now;
            DBGame.Score = PageGame.Score;
            DBGame.CurrentFrame = PageGame.CurrentFrame;
            DBGame.CurrentThrow = PageGame.CurrentThrow;
            DBGame.ScoreUpToFrame = PageGame.ScoreUpToFrame;

            var zip_frames = DBGame.Frames.Zip(PageGame.Frames, (e, n) => new { DB_f = e, Page_f = n });

            foreach (var f in zip_frames)
            {
                f.DB_f.FrameScore = f.Page_f.FrameScore;
                f.DB_f.FrameTotal = f.Page_f.FrameTotal;

                f.DB_f.ThrowOnePins = f.Page_f.ThrowOnePins;
                f.DB_f.ThrowTwoPins = f.Page_f.ThrowTwoPins;
                f.DB_f.ThrowThreePins = f.Page_f.ThrowThreePins;

                f.DB_f.ThrowOneScore = f.Page_f.ThrowOneScore;
                f.DB_f.ThrowTwoScore = f.Page_f.ThrowTwoScore;
                f.DB_f.ThrowThreeScore = f.Page_f.ThrowThreeScore;

            }

            return (DBGame);
        }
    }
}
