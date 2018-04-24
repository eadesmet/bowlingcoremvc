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
            /* questions..
             * when CAN this get executed? can I put my formatting code here?
             * 
             * */
            var content = (await output.GetChildContentAsync()).GetContent();
            //If content == 10, format as 'X' ?????
            //Do I need a Tag helper for every 'Tag' I want to show?

            var tdID = String.Format("{0}_{1}_{2}_tdFrame", frame.GameID, frame.FrameNum, ThrowNum);

            var lblThrowID = String.Format("{0}_{1}_{2}_lblFrame", frame.GameID, frame.FrameNum, ThrowNum);
            var hidThrowID = String.Format("{0}_{1}_{2}_hidFrame", frame.GameID, frame.FrameNum, ThrowNum);

            output.TagName = "td";
            output.Attributes.Add("id", tdID);

            StringBuilder template = new StringBuilder();

            if (ThrowNum == 1)
            {
                template.AppendJoin("", "<label id='", lblThrowID, "'>", frame.ThrowOneScore, "</label>");
                template.AppendJoin("", "<input type='hidden' id='", hidThrowID, "' value='", frame.ThrowOneScore, "'/>");
            }
            else if (ThrowNum == 2)
            {
                template.AppendJoin("", "<label class='frameBox' id='", lblThrowID, "'>", frame.ThrowTwoScore, "</label>");
                template.AppendJoin("", "<input type='hidden' id='", hidThrowID, "' value='", frame.ThrowTwoScore, "'/>");
            }
            else if (ThrowNum == 3)
            {
                template.AppendJoin("", "<label class='frameBox3' id='", lblThrowID, "'>", frame.ThrowThreeScore, "</label>");
                template.AppendJoin("", "<input type='hidden' id='", hidThrowID, "' value='", frame.ThrowThreeScore, "'/>");
            }
            
            output.Content.AppendHtml(template.ToString());

        }
    }
}
