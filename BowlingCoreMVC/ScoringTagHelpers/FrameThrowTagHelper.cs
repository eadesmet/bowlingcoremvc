using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Razor.TagHelpers;
using BowlingCoreMVC.Models;
using System.Text;

namespace BowlingCoreMVC.ScoringTagHelpers
{
    public class FrameThrowTagHelper : TagHelper
    {
        //public int GameID { get; set; }
        //public int FrameNum { get; set; }
        //public int ThrowNum { get; set; }

        public Frame frame { get; set; }
        public int ThrowNum { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);
        }

        //this defaults to Process???
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = (await output.GetChildContentAsync()).GetContent();
            
            var tdID =       String.Format("{0}_{1}_{2}_tdFrame", frame.GameID, frame.FrameNum, ThrowNum);
            var lblThrowID = String.Format("{0}_{1}_{2}_lblFrame", frame.GameID, frame.FrameNum, ThrowNum);
            var hidThrowID = String.Format("{0}_{1}_{2}_hidFrame", frame.GameID, frame.FrameNum, ThrowNum);
            var hidPinsID =  String.Format("{0}_{1}_{2}_hidPins", frame.GameID, frame.FrameNum, ThrowNum);

            var hidFrameNumID = String.Format("{0}_{1}_hidFrameNum", frame.GameID, frame.FrameNum);

            var hidBallID = String.Format("{0}_{1}_{2}_hidBallID", frame.GameID, frame.FrameNum, ThrowNum);
            var hidFeetPos = String.Format("{0}_{1}_{2}_hidFeetPos", frame.GameID, frame.FrameNum, ThrowNum);
            var hidMarkPos = String.Format("{0}_{1}_{2}_hidMarkPos", frame.GameID, frame.FrameNum, ThrowNum);

            //output.TagName = "td";
            //output.Attributes.Add("id", tdID);

            output.TagName = "div";
            output.Attributes.Add("id", tdID);
            output.Attributes.Add("class", "p-1");

            StringBuilder template = new StringBuilder();
            template.AppendJoin("", "<input type='hidden' id='", hidFrameNumID, "' value='", frame.FrameNum, "'/>");

            if (ThrowNum == 1)
            {
                template.AppendJoin("", "<label id='", lblThrowID, "'>", frame.ThrowOneScore, "</label>");
                template.AppendJoin("", "<input type='hidden' id='", hidThrowID, "' value='", frame.ThrowOneScore, "'/>");
                template.AppendJoin("", "<input type='hidden' id='", hidPinsID, "' value='", frame.ThrowOnePins, "'/>");

                template.AppendJoin("", "<input type='hidden' id='", hidBallID, "' value='", frame.BallID, "'/>");
                template.AppendJoin("", "<input type='hidden' id='", hidFeetPos, "' value='", frame.FeetPos, "'/>");
                template.AppendJoin("", "<input type='hidden' id='", hidMarkPos, "' value='", frame.MarkPos, "'/>");
            }
            else if (ThrowNum == 2)
            {
                template.AppendJoin("", "<label class='frameBox' id='", lblThrowID, "'>", frame.ThrowTwoScore, "</label>");
                template.AppendJoin("", "<input type='hidden' id='", hidThrowID, "' value='", frame.ThrowTwoScore, "'/>");
                template.AppendJoin("", "<input type='hidden' id='", hidPinsID, "' value='", frame.ThrowTwoPins, "'/>");

                template.AppendJoin("", "<input type='hidden' id='", hidBallID, "' value='", frame.BallID, "'/>");
                template.AppendJoin("", "<input type='hidden' id='", hidFeetPos, "' value='", frame.FeetPos, "'/>");
                template.AppendJoin("", "<input type='hidden' id='", hidMarkPos, "' value='", frame.MarkPos, "'/>");
            }
            else if (ThrowNum == 3)
            {
                template.AppendJoin("", "<label class='frameBox3' id='", lblThrowID, "'>", frame.ThrowThreeScore, "</label>");
                template.AppendJoin("", "<input type='hidden' id='", hidThrowID, "' value='", frame.ThrowThreeScore, "'/>");
                template.AppendJoin("", "<input type='hidden' id='", hidPinsID, "' value='", frame.ThrowThreePins, "'/>");

                template.AppendJoin("", "<input type='hidden' id='", hidBallID, "' value='", frame.BallID, "'/>");
                template.AppendJoin("", "<input type='hidden' id='", hidFeetPos, "' value='", frame.FeetPos, "'/>");
                template.AppendJoin("", "<input type='hidden' id='", hidMarkPos, "' value='", frame.MarkPos, "'/>");
            }
            
            output.Content.AppendHtml(template.ToString());

        }
    }
}
