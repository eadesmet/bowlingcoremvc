using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Razor.TagHelpers;
using BowlingCoreMVC.Models;
using System.Text;

namespace BowlingCoreMVC.ScoringTagHelpers
{
    public class FrameScoreTagHelper : TagHelper
    {
        public Frame frame { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);
        }

        
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = (await output.GetChildContentAsync()).GetContent();
            
            var tdID = String.Format("{0}_{1}_tdFrameScore", frame.GameID, frame.FrameNum);

            var lblTotalScoreID = String.Format("{0}_{1}_lblFrameTotal", frame.GameID, frame.FrameNum);
            var hidScoreID =      String.Format("{0}_{1}_hidFrameScore", frame.GameID, frame.FrameNum);
            var hidTotalScoreID = String.Format("{0}_{1}_hidFrameTotal", frame.GameID, frame.FrameNum);
            var hidFrameID =      String.Format("{0}_{1}_hidFrameID", frame.GameID, frame.FrameNum);

            output.TagName = "td";
            output.Attributes.Add("id", tdID);
            if (frame.FrameNum != 10)
                output.Attributes.Add("colspan", 2);
            else
                output.Attributes.Add("colspan", 3);

            StringBuilder template = new StringBuilder();

            //template.AppendJoin("", "<label id='", lblTotalScoreID, "'>", frame.FrameTotal, "</label>");
            template.AppendJoin("", "<label id='", lblTotalScoreID, "'>", "</label>");
            template.AppendJoin("", "<input type='hidden' id='", hidScoreID, "' value='", frame.FrameScore, "'/>");
            template.AppendJoin("", "<input type='hidden' id='", hidTotalScoreID, "' value='", frame.FrameTotal, "'/>");
            template.AppendJoin("", "<input type='hidden' id='", hidFrameID, "' value='", frame.ID, "'/>");


            output.Content.AppendHtml(template.ToString());

        }
    }
}
