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

            var tdID = String.Format("{0}_{1}_tdFrame", frame.GameID, frame.FrameNum);

            var lblThrowOneID = String.Format("{0}_{1}_1_lblFrame", frame.GameID, frame.FrameNum);
            var hidThrowOneID = String.Format("{0}_{1}_1_hidFrame", frame.GameID, frame.FrameNum);
            var lblThrowTwoID = String.Format("{0}_{1}_2_lblFrame", frame.GameID, frame.FrameNum);
            var hidThrowTwoID = String.Format("{0}_{1}_2_hidFrame", frame.GameID, frame.FrameNum);
            var lblThrowThreeID = String.Format("{0}_{1}_2_lblFrame", frame.GameID, frame.FrameNum);
            var hidThrowThreeID = String.Format("{0}_{1}_2_hidFrame", frame.GameID, frame.FrameNum);

            output.TagName = "td";
            output.Attributes.Add("id", tdID);
            
            StringBuilder template = new StringBuilder();

            if (frame.FrameNum != 10)
            {
                //TODO: Confirm stringbuilder append
                template.AppendJoin("", "<label id='", lblThrowOneID, "'>", frame.ThrowOneScore, "</label>");
                template.AppendJoin("", "<input type='hidden' id='", hidThrowOneID, "' value='", frame.ThrowOneScore, "'/>");

                template.AppendJoin("", "<label class='frameBox' id='", lblThrowTwoID, "'>", frame.ThrowTwoScore, "</label>");
                template.AppendJoin("", "<input type='hidden' id='", hidThrowTwoID, "' value='", frame.ThrowTwoScore, "'/>");
            }
            else
            {
                template.AppendJoin("", "<label id='", lblThrowOneID, "'>", frame.ThrowOneScore, "</label>");
                template.AppendJoin("", "<input type='hidden' id='", hidThrowOneID, "' value='", frame.ThrowOneScore, "'/>");

                template.AppendJoin("", "<label class='frameBox' id='", lblThrowTwoID, "'>", frame.ThrowTwoScore, "</label>");
                template.AppendJoin("", "<input type='hidden' id='", hidThrowTwoID, "' value='", frame.ThrowTwoScore, "'/>");

                template.AppendJoin("", "<label class='frameBox3' id='", lblThrowThreeID, "'>", frame.ThrowThreeScore, "</label>");
                template.AppendJoin("", "<input type='hidden' id='", hidThrowThreeID, "' value='", frame.ThrowThreeScore, "'/>");
            }

            output.Content.AppendHtml(template.ToString());

        }
    }
}
