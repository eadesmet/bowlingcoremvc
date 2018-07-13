using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Razor.TagHelpers;
using BowlingCoreMVC.Models;
using BowlingCoreMVC.Helpers;
using System.Text;

namespace BowlingCoreMVC.ScoringTagHelpers
{
    public class PinsViewTagHelper : TagHelper
    {
        public Frame frame { get; set; }
        public int ThrowNum { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //base.Process(context, output);

            
            var ID = String.Format("{0}_{1}_PinsView", frame.GameID, frame.FrameNum);

            output.TagName = "table";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add("id", "PinsTable-mini");
            output.Attributes.Add("class", "table table-condensed");


            bool one, two, three, four, five, six, seven, eight, nine, ten;
            if (ThrowNum == 1)
            {
                one = (frame.ThrowOnePins & Pins.MISSED_1) == Pins.MISSED_1;
                two = (frame.ThrowOnePins & Pins.MISSED_2) == Pins.MISSED_2;
                three = (frame.ThrowOnePins & Pins.MISSED_3) == Pins.MISSED_3;
                four = (frame.ThrowOnePins & Pins.MISSED_4) == Pins.MISSED_4;
                five = (frame.ThrowOnePins & Pins.MISSED_5) == Pins.MISSED_5;
                six = (frame.ThrowOnePins & Pins.MISSED_6) == Pins.MISSED_6;
                seven = (frame.ThrowOnePins & Pins.MISSED_7) == Pins.MISSED_7;
                eight = (frame.ThrowOnePins & Pins.MISSED_8) == Pins.MISSED_8;
                nine = (frame.ThrowOnePins & Pins.MISSED_9) == Pins.MISSED_9;
                ten = (frame.ThrowOnePins & Pins.MISSED_10) == Pins.MISSED_10;
            }
            else if (ThrowNum == 2)
            {
                one = (frame.ThrowTwoPins & Pins.MISSED_1) == Pins.MISSED_1;
                two = (frame.ThrowTwoPins & Pins.MISSED_2) == Pins.MISSED_2;
                three = (frame.ThrowTwoPins & Pins.MISSED_3) == Pins.MISSED_3;
                four = (frame.ThrowTwoPins & Pins.MISSED_4) == Pins.MISSED_4;
                five = (frame.ThrowTwoPins & Pins.MISSED_5) == Pins.MISSED_5;
                six = (frame.ThrowTwoPins & Pins.MISSED_6) == Pins.MISSED_6;
                seven = (frame.ThrowTwoPins & Pins.MISSED_7) == Pins.MISSED_7;
                eight = (frame.ThrowTwoPins & Pins.MISSED_8) == Pins.MISSED_8;
                nine = (frame.ThrowTwoPins & Pins.MISSED_9) == Pins.MISSED_9;
                ten = (frame.ThrowTwoPins & Pins.MISSED_10) == Pins.MISSED_10;
            }
            else
            {
                one = (frame.ThrowThreePins & Pins.MISSED_1) == Pins.MISSED_1;
                two = (frame.ThrowThreePins & Pins.MISSED_2) == Pins.MISSED_2;
                three = (frame.ThrowThreePins & Pins.MISSED_3) == Pins.MISSED_3;
                four = (frame.ThrowThreePins & Pins.MISSED_4) == Pins.MISSED_4;
                five = (frame.ThrowThreePins & Pins.MISSED_5) == Pins.MISSED_5;
                six = (frame.ThrowThreePins & Pins.MISSED_6) == Pins.MISSED_6;
                seven = (frame.ThrowThreePins & Pins.MISSED_7) == Pins.MISSED_7;
                eight = (frame.ThrowThreePins & Pins.MISSED_8) == Pins.MISSED_8;
                nine = (frame.ThrowThreePins & Pins.MISSED_9) == Pins.MISSED_9;
                ten = (frame.ThrowThreePins & Pins.MISSED_10) == Pins.MISSED_10;
            }



            StringBuilder template = new StringBuilder();

            //template.AppendJoin("", "<label id='", lblTotalScoreID, "'>", frame.FrameTotal, "</label>");

            template.AppendJoin("", "<tr>");
            template.AppendJoin("", "<td><div><input type='checkbox' class='pins-checkbox-mini pins-checkbox-7 sr-only' ",  seven? "checked" : "",  " disabled /><label for='" + frame.ID + "-Missed-7'></label></div></td>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "<td><div><input type='checkbox' class='pins-checkbox-mini pins-checkbox-8 sr-only' ",  eight? "checked" : "",  " disabled /><label for='" + frame.ID + "-Missed-8'></label></div></td>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "<td><div><input type='checkbox' class='pins-checkbox-mini pins-checkbox-9 sr-only' ",  nine? "checked" : "",  " disabled /><label for='" + frame.ID + "-Missed-9'></label></div></td>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "<td><div><input type='checkbox' class='pins-checkbox-mini pins-checkbox-10 sr-only' ",  ten? "checked" : "",  " disabled /><label for='" + frame.ID + "-Missed-10'></label></div></td>");
            template.AppendJoin("", "</tr>");

            template.AppendJoin("", "<tr>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "<td><div><input type='checkbox' class='pins-checkbox-mini pins-checkbox-4 sr-only' ",  four? "checked" : "",  " disabled /><label for='" + frame.ID + "-Missed-4'></label></div></td>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "<td><div><input type='checkbox' class='pins-checkbox-mini pins-checkbox-5 sr-only' ",  five? "checked" : "",  " disabled /><label for='" + frame.ID + "-Missed-5'></label></div></td>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "<td><div><input type='checkbox' class='pins-checkbox-mini pins-checkbox-6 sr-only' ",  six? "checked" : "",  " disabled /><label for='" + frame.ID + "-Missed-6'></label></div></td>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "</tr>");

            template.AppendJoin("", "<tr>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "<td><div><input type='checkbox' class='pins-checkbox-mini pins-checkbox-2 sr-only' ",  two? "checked" : "",  " disabled /><label for='" + frame.ID + "-Missed-2'></label></div></td>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "<td><div><input type='checkbox' class='pins-checkbox-mini pins-checkbox-3 sr-only' ",  three? "checked" : "",  " disabled /><label for='" + frame.ID + "-Missed-3'></label></div></td>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "</tr>");

            template.AppendJoin("", "<tr>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "<td><div><input type='checkbox' class='pins-checkbox-mini pins-checkbox-1 sr-only' ",  one? "checked" : "",  " disabled /><label for='" + frame.ID + "-Missed-1'></label></div></td>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "<td></td>");
            template.AppendJoin("", "</tr>");

            output.Content.AppendHtml(template.ToString());
            //output.Content.SetContent(template.ToString());

        }

        
        //public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        //{
        //    //var content = (await output.GetChildContentAsync()).GetContent();
        //}
    }
}



/*
 * <table class="table table-condensed" id="PinsTable">
                            <tr>
                                <td><div><input type="checkbox" id="@Model.ID-Missed_7" class="pins-checkbox pins-checkbox-7 sr-only" /><label for="@Model.ID-Missed_7"></label></div></td>
                                <td></td>
                                <td><div><input type="checkbox" id="@Model.ID-Missed_8" class="pins-checkbox pins-checkbox-8 sr-only" /><label for="@Model.ID-Missed_8"></label></div></td>
                                <td></td>
                                <td><div><input type="checkbox" id="@Model.ID-Missed_9" class="pins-checkbox pins-checkbox-9 sr-only" /><label for="@Model.ID-Missed_9"></label></div></td>
                                <td></td>
                                <td><div><input type="checkbox" id="@Model.ID-Missed_10" class="pins-checkbox pins-checkbox-10 sr-only" /><label for="@Model.ID-Missed_10"></label></div></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td><div><input type="checkbox" id="@Model.ID-Missed_4" class="pins-checkbox pins-checkbox-4 sr-only" /><label for="@Model.ID-Missed_4"></label></div></td>
                                <td></td>
                                <td><div><input type="checkbox" id="@Model.ID-Missed_5" class="pins-checkbox pins-checkbox-7 sr-only" /><label for="@Model.ID-Missed_5"></label></div></td>
                                <td></td>
                                <td><div><input type="checkbox" id="@Model.ID-Missed_6" class="pins-checkbox pins-checkbox-6 sr-only" /><label for="@Model.ID-Missed_6"></label></div></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                                <td><div><input type="checkbox" id="@Model.ID-Missed_2" class="pins-checkbox pins-checkbox-2 sr-only" /><label for="@Model.ID-Missed_2"></label></div></td>
                                <td></td>
                                <td><div><input type="checkbox" id="@Model.ID-Missed_3" class="pins-checkbox pins-checkbox-3 sr-only" /><label for="@Model.ID-Missed_3"></label></div></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td><div><input type="checkbox" id="@Model.ID-Missed_1" class="pins-checkbox pins-checkbox-1 sr-only" /><label for="@Model.ID-Missed_1"></label></div></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                        </table>
 * 
 * */