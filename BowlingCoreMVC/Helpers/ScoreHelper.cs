using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BowlingCoreMVC.Models;

namespace BowlingCoreMVC.Helpers
{
    public static class ScoreHelper
    {
        static List<Frame> tempFramesList;


        //TODO: I don't like the throwcurrent logic, try to refactor


        //public. Called when next button was clicked.
        public static Game ThrowCurrent(Game g)
        {
            tempFramesList = g.Frames.ToList();
            int ThrowOneScore = tempFramesList[g.CurrentFrame - 1].ThrowOneScore;

            g = CalculateNext(g, ThrowOneScore);

            g = ScoreGame(g);

            if (g.ScoreUpToFrame < g.CurrentFrame)
                g.ScoreUpToFrame = g.CurrentFrame;

            return (g);
        }

        //Private. Called when calculating throw, moving forward
        private static Game CalculateNext(Game g, int ThrowOneScore)
        {
            int CurrentThrowNum = GetThrowNum(g.CurrentThrow);
            //----Recalculate CurrentFrame----
            switch (CurrentThrowNum)
            {
                case 1:
                    if (g.CurrentFrame != 10 && ThrowOneScore == 10)
                    {
                        g.CurrentFrame++;
                        //g.ScoreUpToFrame = g.CurrentFrame - 1;
                    }
                    break;
                case 2:
                    if (g.CurrentFrame != 10)
                    {
                        g.CurrentFrame++;
                        //g.ScoreUpToFrame = g.CurrentFrame;
                    }
                    break;
                case 3:
                    break;
            }

            //if (g.CurrentFrame == 10)
            //    g.ScoreUpToFrame = 10;

            return (CalculateCurrentThrow(g, ThrowOneScore, true));
        }

        //public. Called when previous button was clicked.
        public static Game CalculatePrevious(Game g)
        {
            //Previous is ONLY going backwards
            //No scoring, no special rules
            int CurrentThrowNum = GetThrowNum(g.CurrentThrow);
            if (CurrentThrowNum == 1 && g.CurrentFrame != 1)
                g.CurrentFrame--;

            return (CalculateCurrentThrow(g, 0, false));
        }

        /// <summary>
        /// Recalculates the CurrentThrow based on ThrowOneScore and g.CurrentFrame
        /// Can be called going forward or backward using 'forward' bool
        /// </summary>
        /// <param name="g"></param>
        /// <param name="ThrowOneScore"></param>
        /// <param name="forward"></param>
        private static Game CalculateCurrentThrow(Game g, int ThrowOneScore, bool forward)
        {
            int CurrentThrowNum = GetThrowNum(g.CurrentThrow);

            //NOTE: Remember currentFrame is already changed.
            //----Recalculate CurrentThrow----
            if (g.CurrentFrame < 10)
            {
                //If Throw was First ball
                if (CurrentThrowNum == 1 && ThrowOneScore != 10)
                {
                    /*
                    if (!forward && g.CurrentFrame == 1)
                    { 
                        g.CurrentThrow = (g.CurrentFrame * 2) - 1;
                        return;
                    }
                    */
                    //Set throw to second ball
                    g.CurrentThrow = (g.CurrentFrame * 2);
                }
                else
                {
                    g.CurrentThrow = (g.CurrentFrame * 2) - 1;
                }

            }
            else
            {
                //10th frame
                if (forward)
                {
                    switch (g.CurrentThrow)
                    {
                        case 19:
                            g.CurrentThrow++;
                            break;
                        case 20:
                            var tempFramesList = g.Frames.ToList();
                            if (ThrowOneScore + tempFramesList[g.CurrentFrame - 1].ThrowTwoScore >= 10)
                            {
                                g.CurrentThrow++;
                            }
                            break;
                        case 21:
                            break;
                        default:
                            g.CurrentThrow = 19;
                            break;
                    }
                }
                else
                {
                    if (g.CurrentThrow != 19)
                    {
                        g.CurrentThrow--;
                    }
                }
            }
            return (g);
        }

        private static int GetThrowNum(int CurrentThrow)
        {
            //CurrentThrow passed in is 1-21 throws of the game.
            //This converts it to 1-3 for the throw of the frame
            int result = CurrentThrow % 2;
            if (result == 0)
                return (2);
            else
                if (CurrentThrow == 21)
                return (3);
            else
                return (1);
        }

        //-----------------------------------------------------------------

        public static Game ScoreGame(Game g)
        {
            int score = 0;
            var tempFramesList = g.Frames.ToList();
            //TODO: Confirm this is IN ORDER
            foreach (var f in g.Frames)
            {
                //if (g.ScoreUpToFrame >= g.CurrentFrame)
                //    break;

                if (f.ThrowOneScore == 10)
                {
                    f.FrameScore = StrikeBonus(tempFramesList, f.FrameNum - 1);
                }
                else if (f.ThrowOneScore + f.ThrowTwoScore == 10)
                {
                    f.FrameScore = SpareBonus(tempFramesList, f.FrameNum - 1);
                }
                else
                {
                    f.FrameScore = f.ThrowOneScore + f.ThrowTwoScore;
                }

                //Blank out 10th frame 3rd throw if no spare/strike
                if ((f.FrameNum == 10) && (f.ThrowOneScore + f.ThrowTwoScore < 10))
                    f.ThrowThreeScore = 0;

                //Blank out 2nd throw if first is strike (but not 10th)
                if (f.ThrowOneScore == 10 && f.FrameNum != 10)
                    f.ThrowTwoScore = 0;

                score += f.FrameScore;
                f.FrameTotal = score;
            }

            g.Score = score;
            return (g);
        }

        private static int StrikeBonus(List<Frame> frameslist, int frameIndex)//(Game g, int frameIndex)
        {
            //TODO: Confrim this list is IN ORDER!
            //var frameslist = g.Frames.ToList();

            if (frameIndex == 8) //strike 9th frame
            {
                //var a = g.Frames.Where(o => o.FrameNum == frame.FrameNum + 1).SingleOrDefault().ThrowOneScore;
                //FrameNum 9 (9th frame) get index 9 (10th frame)


                return (10 + frameslist[9].ThrowOneScore + frameslist[9].ThrowTwoScore); // frame.Next.ThrowOne.Points + frame.Next.ThrowTwo.Points);
            }
            else if (frameIndex == 9) //10th
            {
                return (frameslist[frameIndex].ThrowOneScore + frameslist[frameIndex].ThrowTwoScore + frameslist[frameIndex].ThrowThreeScore);
            }
            else //frames 1-8
            {
                if (frameslist[frameIndex + 1].ThrowOneScore == 10)
                {
                    return (10 + frameslist[frameIndex + 1].ThrowOneScore + frameslist[frameIndex + 2].ThrowOneScore);
                }
                else
                {
                    return (10 + frameslist[frameIndex + 1].ThrowOneScore + frameslist[frameIndex + 1].ThrowTwoScore);
                }
            }
        }

        private static int SpareBonus(List<Frame> frameslist, int frameIndex)
        {
            if (frameslist[frameIndex].FrameNum == 10)
            {
                return (10 + frameslist[frameIndex].ThrowThreeScore);
            }
            else
            {
                return (10 + frameslist[frameIndex + 1].ThrowOneScore);
            }
        }
    }

    public static class Pins
    {
        public const short MISSED_10 = 512;   // 0000 0010 0000 0000
        public const short MISSED_9 = 256;    // 0000 0001 0000 0000
        public const short MISSED_8 = 128;    // 0000 0000 1000 0000
        public const short MISSED_7 = 64;     // 0000 0000 0100 0000
        public const short MISSED_6 = 32;     // 0000 0000 0010 0000
        public const short MISSED_5 = 16;     // 0000 0000 0001 0000
        public const short MISSED_4 = 8;      // 0000 0000 0000 1000
        public const short MISSED_3 = 4;      // 0000 0000 0000 0100
        public const short MISSED_2 = 2;      // 0000 0000 0000 0010
        public const short MISSED_1 = 1;      // 0000 0000 0000 0001
        public const short MISSED_0 = 0;      // 0000 0000 0000 0000
        public const short MISSED_ALL = 1023; // 0000 0011 1111 1111

    }
}
