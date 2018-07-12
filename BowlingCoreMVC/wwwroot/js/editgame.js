// Write your JavaScript code.


/*

ID Format
FrameThrows
    {0}_{1}_{2}_tdFrame  (frame.GameID, frame.FrameNum, ThrowNum) [TD]
    {0}_{1}_{2}_lblFrame (frame.GameID, frame.FrameNum, ThrowNum) [LABEL]
    {0}_{1}_{2}_hidFrame (frame.GameID, frame.FrameNum, ThrowNum) [HIDDEN]

FrameScore
    {0}_{1}_tdFrameScore  (GameID, FrameNum) [TD]
    {0}_{1}_lblFrameTotal (GameID, FrameNum) [LABEL]
    {0}_{1}_hidFrameScore (GameID, FrameNum) [HIDDEN]
    {0}_{1}_hidFrameTotal (GameID, FrameNum) [HIDDEN]

Missed Pins
    {0}-Missed_{1} (GameID, Pin#) [CHECKBOX]
    {0}_{1}_{2}_hidPins (frame.GameID, frame.FrameNum, ThrowNum) [HIDDEN]

Misc
    {0}_CurrentFrame (GameID)
    {0}_CurrentThrow (GameID)
    {0}_ScoreUpToFrame (GameID)
    {0}_{1}_FrameID  (GameID, FrameNum)

*/

///Getting GameData from the page (labels) to be sent to Controller
///Returns JSON string of the Game
function GetJSONFromPage(GameID)
{
    var game = {};
    game.Frames = [];
    game.ID = GameID;
    //game.CreatedDate = $("#game_CreatedDate").val();
    //game.ModifiedDate = $("#game_ModifiedDate").val();
    game.CurrentFrame = $("#" + GameID + "_CurrentFrame").val();
    game.CurrentThrow = $("#" + GameID + "_CurrentThrow").val();
    game.ScoreUpToFrame = $("#" + GameID + "_ScoreUpToFrame").val();
    //game.UserID = '@Model.UserID';

    //SetPrevious(game);
    ClearHighlight(GameID, parseInt(game.CurrentFrame));

    for (var i = 1; i <= 10; i++)
    {
        var frameDetails = {};

        var prefixID = "#" + GameID + "_" + i + "_";

        frameDetails["GameID"] = GameID;
        frameDetails["ID"] = $(prefixID + "FrameID").val();

        frameDetails["FrameScore"] = $(prefixID + "hidFrameScore").val();
        frameDetails["FrameTotal"] = $(prefixID + "hidFrameTotal").val();
        frameDetails["FrameNum"] = $(prefixID + "hidFrameNum").val();


        frameDetails["ThrowOneScore"] = $(prefixID + "1_hidFrame").val();
        frameDetails["ThrowTwoScore"] = $(prefixID + "2_hidFrame").val();

        frameDetails["ThrowOnePins"] = $(prefixID + "1_hidPins").val();
        frameDetails["ThrowTwoPins"] = $(prefixID + "2_hidPins").val();


        //TODO: 10th frame updating here
        if (i === 10)
        {
            frameDetails["ThrowThreeScore"] = $(prefixID + "3_hidFrame").val();
            frameDetails["ThrowThreePins"] = $(prefixID + "3_hidPins").val();
        }

        game.Frames.push(frameDetails);
    }
    return JSON.stringify(game);
}

function UpdateCurrentThrowVal(GameID)
{
    //Update the hidden values for the score/pins of the current throw
    var CurrentThrow = parseInt($("#" + GameID + "_CurrentThrow").val());
    var CurrentFrame = parseInt($("#" + GameID + "_CurrentFrame").val());
    var prefixID = "#" + GameID + "_" + CurrentFrame + "_";
    var ThrowNum = parseInt(GetThrowNum(parseInt(CurrentThrow)));

    var CurrentThrowData = GetThrowPins(GameID, CurrentFrame, ThrowNum);
    //(might be overwritten in refresh game? make sure framedetails is getting updated)
    $(prefixID + ThrowNum + "_hidFrame").val(CurrentThrowData.score);
    $(prefixID + ThrowNum + "_hidPins").val(CurrentThrowData.missed);

    if (CurrentFrame === 10 && ThrowNum === 1)
    {
        //Reset the 10th frame when setting the first ball
        //$(prefixID + 2 + "_hidFrame").val(0);
        //$(prefixID + 2 + "_hidPins").val(0);
        //$(prefixID + 3 + "_hidFrame").val(0);
        //$(prefixID + 3 + "_hidPins").val(0);
    }

    //Overwrite second throw score to be calculated from throw one score
    if (ThrowNum === 2)
    {
        //NOTE: GetThrowPins gets the score from the pins only! getThrowHidVals gets any score that has been saved
        var FirstThrowData = GetThrowHidVals(GameID, CurrentFrame, 1);
        //$(prefixID + ThrowNum + "_hidFrame").val(CurrentThrowData.score - FirstThrowData.score);

        if (CurrentFrame === 10)
        {
            if (FirstThrowData.score === 10)
            {
                //first ball of 10th is strike, second ball is the current score only (not sub from first)
                $(prefixID + ThrowNum + "_hidFrame").val(CurrentThrowData.score);
            }
        }
    }

    if (ThrowNum === 3)
    {
        //last ball 10th frame. if we are here, just take what the pins are as it's score
        //current throw is third..

        var FirstThrowData = GetThrowHidVals(GameID, CurrentFrame, 1);
        var TenthSecondThrowData = GetThrowHidVals(GameID, CurrentFrame, 2);
        //if (TenthSecondThrowData.score < 10)

        if (FirstThrowData.score == 10 && TenthSecondThrowData.score < 10)
        {
            $(prefixID + ThrowNum + "_hidFrame").val(CurrentThrowData.score - TenthSecondThrowData.score);
        }
        else
        {
            $(prefixID + ThrowNum + "_hidFrame").val(CurrentThrowData.score);
        }

        //10th frame scenerios
        //first ball strike, second ball fresh
        //first ball strike, second ball strike, third ball fresh
        //first ball strike, second ball not strike, third ball from second
        //first + second spare, third ball fresh
        //forst + second not spare, no third ball

    }
}


///Goes through the checkboxes and gets the Integer of missed pins
function GetThrowPins(GameID, frameNum, throwNum)
{
    var missed_pins = 0;
    var throw_score = 10;

    var one = $("#" + GameID + "-Missed_1");
    var two = $("#" + GameID + "-Missed_2");
    var three = $("#" + GameID + "-Missed_3");
    var four = $("#" + GameID + "-Missed_4");
    var five = $("#" + GameID + "-Missed_5");
    var six = $("#" + GameID + "-Missed_6");
    var seven = $("#" + GameID + "-Missed_7");
    var eight = $("#" + GameID + "-Missed_8");
    var nine = $("#" + GameID + "-Missed_9");
    var ten = $("#" + GameID + "-Missed_10");


    if (one.is(':checked') || one.is(':disabled')) { throw_score = throw_score - 1; }
    if (two.is(':checked') || two.is(':disabled')) { throw_score = throw_score - 1; }
    if (three.is(':checked') || three.is(':disabled')) { throw_score = throw_score - 1; }
    if (four.is(':checked') || four.is(':disabled')) { throw_score = throw_score - 1; }
    if (five.is(':checked') || five.is(':disabled')) { throw_score = throw_score - 1; }
    if (six.is(':checked') || six.is(':disabled')) { throw_score = throw_score - 1; }
    if (seven.is(':checked') || seven.is(':disabled')) { throw_score = throw_score - 1; }
    if (eight.is(':checked') || eight.is(':disabled')) { throw_score = throw_score - 1; }
    if (nine.is(':checked') || nine.is(':disabled')) { throw_score = throw_score - 1; }
    if (ten.is(':checked') || ten.is(':disabled')) { throw_score = throw_score - 1; }

    if (one.is(':checked')) { missed_pins = missed_pins + MISSED_1; }
    if (two.is(':checked')) { missed_pins = missed_pins + MISSED_2; }
    if (three.is(':checked')) { missed_pins = missed_pins + MISSED_3; }
    if (four.is(':checked')) { missed_pins = missed_pins + MISSED_4; }
    if (five.is(':checked')) { missed_pins = missed_pins + MISSED_5; }
    if (six.is(':checked')) { missed_pins = missed_pins + MISSED_6; }
    if (seven.is(':checked')) { missed_pins = missed_pins + MISSED_7; }
    if (eight.is(':checked')) { missed_pins = missed_pins + MISSED_8; }
    if (nine.is(':checked')) { missed_pins = missed_pins + MISSED_9; }
    if (ten.is(':checked')) { missed_pins = missed_pins + MISSED_10; }


//    alert($("#" + GameID + "-Missed_10").is(':disabled'));
//    alert($("#" + GameID + "-Missed_10").is(':disabled') || $("#" + GameID + "-Missed_10").is(':checked'));
    //Disabled OR Checked = Subtract from the score



    //if (one) { missed_pins = missed_pins + MISSED_1; throw_score = throw_score - 1; }
    //if (two) { missed_pins = missed_pins + MISSED_2; throw_score = throw_score - 1; }
    //if (three) { missed_pins = missed_pins + MISSED_3; throw_score = throw_score - 1; }
    //if (four) { missed_pins = missed_pins + MISSED_4; throw_score = throw_score - 1; }
    //if (five) { missed_pins = missed_pins + MISSED_5; throw_score = throw_score - 1; }
    //if (six) { missed_pins = missed_pins + MISSED_6; throw_score = throw_score - 1; }
    //if (seven) { missed_pins = missed_pins + MISSED_7; throw_score = throw_score - 1; }
    //if (eight) { missed_pins = missed_pins + MISSED_8; throw_score = throw_score - 1; }
    //if (nine) { missed_pins = missed_pins + MISSED_9; throw_score = throw_score - 1; }
    //if (ten) { missed_pins = missed_pins + MISSED_10; throw_score = throw_score - 1; }

    return { missed: missed_pins, score: throw_score };
}

function GetThrowHidVals(GameID, frameNum, throwNum)
{
    var score = parseInt($("#" + GameID + "_" + frameNum + "_" + throwNum + "_hidFrame").val());
    var missed = parseInt($("#" + GameID + "_" + frameNum + "_" + throwNum + "_hidPins").val());
    return { missed: missed, score: score };
}

//------------------Refresh functions------------------
///This only updates the hidden fields for the game passed in
function RefreshGameHid(g)
{
    $("#" + g.ID + "_CurrentFrame").val(g.CurrentFrame);
    $("#" + g.ID + "_CurrentThrow").val(g.CurrentThrow);
    $("#" + g.ID + "_ScoreUpToFrame").val(g.ScoreUpToFrame);

    for (var i = 1; i <= 10; i++)
    {
        $("#" + g.ID + "_" + i + "_hidFrameScore").val(g.Frames[i - 1].FrameScore);
        $("#" + g.ID + "_" + i + "_hidFrameTotal").val(g.Frames[i - 1].FrameTotal);
        $("#" + g.ID + "_" + i + "_1_hidFrame").val(g.Frames[i - 1].ThrowOneScore);
        $("#" + g.ID + "_" + i + "_2_hidFrame").val(g.Frames[i - 1].ThrowTwoScore);
        $("#" + g.ID + "_" + i + "_1_hidPins").val(g.Frames[i - 1].ThrowOnePins);
        $("#" + g.ID + "_" + i + "_2_hidPins").val(g.Frames[i - 1].ThrowTwoPins);

        if (i === 10)
        {
            $("#" + g.ID + "_" + i + "_3_hidFrame").val(g.Frames[i - 1].ThrowThreeScore);
            $("#" + g.ID + "_" + i + "_3_hidPins").val(g.Frames[i - 1].ThrowThreePins);
        }

        if (i < parseInt(g.ScoreUpToFrame) || parseInt(g.ScoreUpToFrame) === 10)
        {
            SetLabelsFromHid(g.ID, i);
        }
        FormatFrameThrowLabels(g.ID, i);
        // : MissedPinsINT Bitwise & MISSED_1 : (In javascript, this should return 1 if it was missed)
        //$("#" + i + "_1_MissedPinOne").prop("checked", (g.Frames[i - 1].ThrowOnePins & MISSED_1) === MISSED_1);

    }

    var CurrentThrowMissedPins = $("#" + g.ID + "_" + g.CurrentFrame + "_" + GetThrowNum(g.CurrentThrow) + "_hidPins").val();

    RefreshPinsOfCurrentThrow(g.ID, CurrentThrowMissedPins, g.CurrentFrame, g.CurrentThrow, g.ScoreUpToFrame);
}

//------------------------Updating Pins---------------------------------------
function RefreshPinsOfCurrentThrow(GameID, MissedPins, FrameNum, CurrentThrow, ScoreUpToFrame)
{
    const FirstThrows = new Set([1, 3, 5, 7, 9, 11, 13, 15, 17, 19]);
    const SecondThrows = new Set([2, 4, 6, 8, 10, 12, 14, 16, 18]);
    const Tenth = new Set([20, 21]);
    //NOTE: This will be null if CurrentThrow === 1, but we don't use it then anyway
    var PrevMissedPins = parseInt($("#" + GameID + "_" + FrameNum + "_" + (GetThrowNum(CurrentThrow - 1)) + "_hidPins").val());

    //reset (enable all)
    EnableAllMissedPins(GameID);

    //ensure CurrentThrow is an int (.has doesn't work otherwise)
    CurrentThrow = parseInt(CurrentThrow);
    ScoreUpToFrame = parseInt(ScoreUpToFrame);

    if (FirstThrows.has(CurrentThrow))
    {
        //all enabled, set saved missed
        SetMissedPinsChecked(GameID, MissedPins);
    }
    else if (SecondThrows.has(CurrentThrow))
    {
        //previous missed enabled
        SetMissedPinsEnabled(GameID, PrevMissedPins);

        if (ScoreUpToFrame >= FrameNum)
        {
            //set saved missed
            SetMissedPinsChecked(GameID, MissedPins);
        }
        else
        {
            //set previous missed
            SetMissedPinsChecked(GameID, PrevMissedPins);
        }
    }
    else if (Tenth.has(CurrentThrow))
    {
        if (PrevMissedPins === 0)
        {
            //all enabled, set saved missed
            SetMissedPinsChecked(GameID, MissedPins);
        }
        else
        {
            //previous missed enabled, set saved missed
            SetMissedPinsEnabled(GameID, PrevMissedPins);
            SetMissedPinsChecked(GameID, MissedPins);
        }
    }
}

function SetMissedPinsChecked(GameID, MissedPins)
{
    $("#" + GameID + "-Missed_1").prop("checked", (MissedPins & MISSED_1) === MISSED_1);
    $("#" + GameID + "-Missed_2").prop("checked", (MissedPins & MISSED_2) === MISSED_2);
    $("#" + GameID + "-Missed_3").prop("checked", (MissedPins & MISSED_3) === MISSED_3);
    $("#" + GameID + "-Missed_4").prop("checked", (MissedPins & MISSED_4) === MISSED_4);
    $("#" + GameID + "-Missed_5").prop("checked", (MissedPins & MISSED_5) === MISSED_5);
    $("#" + GameID + "-Missed_6").prop("checked", (MissedPins & MISSED_6) === MISSED_6);
    $("#" + GameID + "-Missed_7").prop("checked", (MissedPins & MISSED_7) === MISSED_7);
    $("#" + GameID + "-Missed_8").prop("checked", (MissedPins & MISSED_8) === MISSED_8);
    $("#" + GameID + "-Missed_9").prop("checked", (MissedPins & MISSED_9) === MISSED_9);
    $("#" + GameID + "-Missed_10").prop("checked", (MissedPins & MISSED_10) === MISSED_10);
}

function SetMissedPinsEnabled(GameID, MissedPins)
{
    //MissedPins here should be the previous frames missed pins (also, disabled false === enabled)
    $("#" + GameID + "-Missed_1").prop("disabled", (MissedPins & MISSED_1) !== MISSED_1);
    $("#" + GameID + "-Missed_2").prop("disabled", (MissedPins & MISSED_2) !== MISSED_2);
    $("#" + GameID + "-Missed_3").prop("disabled", (MissedPins & MISSED_3) !== MISSED_3);
    $("#" + GameID + "-Missed_4").prop("disabled", (MissedPins & MISSED_4) !== MISSED_4);
    $("#" + GameID + "-Missed_5").prop("disabled", (MissedPins & MISSED_5) !== MISSED_5);
    $("#" + GameID + "-Missed_6").prop("disabled", (MissedPins & MISSED_6) !== MISSED_6);
    $("#" + GameID + "-Missed_7").prop("disabled", (MissedPins & MISSED_7) !== MISSED_7);
    $("#" + GameID + "-Missed_8").prop("disabled", (MissedPins & MISSED_8) !== MISSED_8);
    $("#" + GameID + "-Missed_9").prop("disabled", (MissedPins & MISSED_9) !== MISSED_9);
    $("#" + GameID + "-Missed_10").prop("disabled", (MissedPins & MISSED_10) !== MISSED_10);
}

function EnableAllMissedPins(GameID)
{
    $("#" + GameID + "-Missed_1").prop("disabled", false);
    $("#" + GameID + "-Missed_2").prop("disabled", false);
    $("#" + GameID + "-Missed_3").prop("disabled", false);
    $("#" + GameID + "-Missed_4").prop("disabled", false);
    $("#" + GameID + "-Missed_5").prop("disabled", false);
    $("#" + GameID + "-Missed_6").prop("disabled", false);
    $("#" + GameID + "-Missed_7").prop("disabled", false);
    $("#" + GameID + "-Missed_8").prop("disabled", false);
    $("#" + GameID + "-Missed_9").prop("disabled", false);
    $("#" + GameID + "-Missed_10").prop("disabled", false);
}


//-----------------------Setting Labels---------------------
function SetLabelsFromHid(GameID, FrameNum)
{
    EmptyFrameLabels(GameID, FrameNum);

    $("#" + GameID + "_" + FrameNum + "_lblFrameTotal").append($("#" + GameID + "_" + FrameNum + "_hidFrameTotal").val());
    //$("#" + GameID + "_" + FrameNum + "_1_lblFrame").append($("#" + GameID + "_" + FrameNum + "_1_hidFrame").val());
    //$("#" + GameID + "_" + FrameNum + "_2_lblFrame").append($("#" + GameID + "_" + FrameNum + "_2_hidFrame").val());

    //FormatFrameThrowLabels(GameID, FrameNum);

    //if (FrameNum === 10)
    //{
    //    $("#" + GameID + "_" + FrameNum + "_3_lblFrame").append($("#" + GameID + "_" + FrameNum + "_3_hidFrame").val());
    //}

}

function EmptyFrameLabels(GameID, FrameNum)
{
    $("#" + GameID + "_" + FrameNum + "_lblFrameTotal").empty();
    $("#" + GameID + "_" + FrameNum + "_1_lblFrame").empty();
    $("#" + GameID + "_" + FrameNum + "_2_lblFrame").empty();
    if (FrameNum === 10)
    {
        $("#" + GameID + "_" + FrameNum + "_3_lblFrame").empty();
    }
}

function FormatFrameThrowLabels(GameID, FrameNum)
{
    var FirstThrowScore = parseInt($("#" + GameID + "_" + FrameNum + "_1_hidFrame").val());
    var SecondThrowScore = parseInt($("#" + GameID + "_" + FrameNum + "_2_hidFrame").val());
    var ScoreUpToFrame = parseInt($("#" + GameID + "_ScoreUpToFrame").val());

    if (FrameNum !== 10)
    {
        if (FirstThrowScore === 10)
        {
            $("#" + GameID + "_" + FrameNum + "_2_lblFrame").empty().append("X");
        }
        else if (FirstThrowScore + SecondThrowScore === 10)
        {
            $("#" + GameID + "_" + FrameNum + "_1_lblFrame").empty().append(FirstThrowScore);
            $("#" + GameID + "_" + FrameNum + "_2_lblFrame").empty().append("/");
        }
        else
        {
            $("#" + GameID + "_" + FrameNum + "_1_lblFrame").empty().append(FirstThrowScore);
            $("#" + GameID + "_" + FrameNum + "_2_lblFrame").empty().append(SecondThrowScore);
        }
    }
    else
    {
        var ThirdThrowScore = parseInt($("#" + GameID + "_" + FrameNum + "_3_hidFrame").val());
        if (FirstThrowScore === 10)
        {
            $("#" + GameID + "_" + FrameNum + "_1_lblFrame").empty().append("X");
            if (SecondThrowScore === 10)
            {
                $("#" + GameID + "_" + FrameNum + "_2_lblFrame").empty().append("X");
            }
            else
            {
                $("#" + GameID + "_" + FrameNum + "_2_lblFrame").empty().append(SecondThrowScore);
            }
        }
        else if (FirstThrowScore + SecondThrowScore === 10)
        {
            $("#" + GameID + "_" + FrameNum + "_1_lblFrame").empty().append(FirstThrowScore);
            $("#" + GameID + "_" + FrameNum + "_2_lblFrame").empty().append("/");
        }
        else
        {
            $("#" + GameID + "_" + FrameNum + "_1_lblFrame").empty().append(FirstThrowScore);
            $("#" + GameID + "_" + FrameNum + "_2_lblFrame").empty().append(SecondThrowScore);
        }

        if (ThirdThrowScore === 10)
        {
            $("#" + GameID + "_" + FrameNum + "_3_lblFrame").empty().append("X");
        }
        else if (SecondThrowScore + ThirdThrowScore === 10)
        {
            //$("#" + GameID + "_" + FrameNum + "_2_lblFrame").append(SecondThrowScore);
            $("#" + GameID + "_" + FrameNum + "_3_lblFrame").empty().append("/");
        }
        else
        {
            $("#" + GameID + "_" + FrameNum + "_3_lblFrame").empty().append(ThirdThrowScore);
        }

        if (ThirdThrowScore === 0)
        {
            if (ScoreUpToFrame < FrameNum)
            {
                $("#" + GameID + "_" + FrameNum + "_3_lblFrame").empty();
            }
            else
            {
                $("#" + GameID + "_" + FrameNum + "_3_lblFrame").empty().append("-");
            }

        }
    }

    if (FirstThrowScore === 0)
    {
        if (ScoreUpToFrame < FrameNum)
        {
            $("#" + GameID + "_" + FrameNum + "_1_lblFrame").empty();
        }
        else
        {
            $("#" + GameID + "_" + FrameNum + "_1_lblFrame").empty().append("-");
        }
    }

    if (SecondThrowScore === 0 && FirstThrowScore !== 10)
    {
        if (ScoreUpToFrame < FrameNum)
        {
            $("#" + GameID + "_" + FrameNum + "_2_lblFrame").empty();
        }
        else
        {
            $("#" + GameID + "_" + FrameNum + "_2_lblFrame").empty().append("-");
        }
    }

}


const MISSED_10 = 512;   // 0000 0010 0000 0000
const MISSED_9 = 256;    // 0000 0001 0000 0000
const MISSED_8 = 128;    // 0000 0000 1000 0000
const MISSED_7 = 64;     // 0000 0000 0100 0000
const MISSED_6 = 32;     // 0000 0000 0010 0000
const MISSED_5 = 16;     // 0000 0000 0001 0000
const MISSED_4 = 8;      // 0000 0000 0000 1000
const MISSED_3 = 4;      // 0000 0000 0000 0100
const MISSED_2 = 2;      // 0000 0000 0000 0010
const MISSED_1 = 1;      // 0000 0000 0000 0001
const MISSED_0 = 0;      // 0000 0000 0000 0000
const MISSED_ALL = 1023; // 0000 0011 1111 1111

//----------Highlight functions-------------
function ClearHighlight(GameID, FrameNum)
{
    var prefixID = "#" + GameID + "_" + FrameNum + "_";

    $(prefixID + "1_tdFrame").css("background-color", "White");
    $(prefixID + "2_tdFrame").css("background-color", "White");
    $(prefixID + "tdFrameScore").css("background-color", "White");

    if (FrameNum === 10 || FrameNum === 9)
    {
        $("#" + GameID + "_10_3_tdFrame").css("background-color", "White");
    }
}

function HighlightSelectedFrame(GameID, FrameNum, CurrentThrow)
{
    //TODO: Refactor this a bit. Can get ThrowNum and condense this a lot
    //NOTE: Might not need CurrentThrow, we can get it ourselves here
    var prefixID = "#" + GameID + "_" + FrameNum + "_";

    $(prefixID + "1_tdFrame").css("background-color", "Aquamarine");
    $(prefixID + "2_tdFrame").css("background-color", "Aquamarine");
    $(prefixID + "tdFrameScore").css("background-color", "Aquamarine");
    if (FrameNum === 10)
    {
        $(prefixID + "3_tdFrame").css("background-color", "Aquamarine");
        if (CurrentThrow === 21)
        {
            $(prefixID + "3_tdFrame").css("background-color", "Plum");
        }
        else
        {
            var fThrow = CurrentThrow % 2;
            if (fThrow === 0) { fThrow = 2; }
            if (fThrow === 1)
            {
                $(prefixID + "1_tdFrame").css("background-color", "Plum");
            }
            else if (fThrow === 2)
            {
                $(prefixID + "2_tdFrame").css("background-color", "Plum");
            }
        }
    }
    else
    {
        var fThrow = CurrentThrow % 2;
        if (fThrow === 0) { fThrow = 2; }
        if (fThrow === 1)
        {
            $(prefixID + "1_tdFrame").css("background-color", "Plum");
        }
        else if (fThrow === 2)
        {
            $(prefixID + "2_tdFrame").css("background-color", "Plum");
        }
    }
}


// TODO: Does this get called on EVERY page?
//Document Load
$(document).ready(function ()
{
    var GameIDs = GetGameIDs();
    for (var i = 0; i < GameIDs.length; i++)
    {
        //look up the current game / frame
        var CurrentFrame = parseInt($("#" + GameIDs[i] + "_CurrentFrame").val());
        var CurrentThrow = parseInt($("#" + GameIDs[i] + "_CurrentThrow").val());


        //var ThrowNum = GetThrowNum(CurrentThrow);

        //var MissedPins = $("#" + GameIDs[i] + "_" + CurrentFrame + "_" + ThrowNum + "_hidPins").val();
        //var ScoreUpTo = $("#" + GameIDs[i] + "_ScoreUpToFrame").val();

        //RefreshPinsOfCurrentThrow(GameIDs[i], MissedPins, CurrentFrame, CurrentThrow, ScoreUpTo);
        RefreshGameHid(JSON.parse(GetJSONFromPage(GameIDs[i])));

        HighlightSelectedFrame(GameIDs[i], CurrentFrame, CurrentThrow);
    }
});

function MissedAllClick(GameID)
{
    MissedAllClick.toggle = !MissedAllClick.toggle;
    if (MissedAllClick.toggle)
    {
        SetMissedPinsChecked(GameID, MISSED_ALL);
    }
    else
    {
        SetMissedPinsChecked(GameID, MISSED_0);
    }
}

function MissedNoneClick(GameID)
{
    SetMissedPinsChecked(GameID, MISSED_0);
}

//---------------------------------------
//----- AJAX CALLS ----------------------
//---------------------------------------

function NextClickSendGame(GameID)
{
    UpdateCurrentThrowVal(GameID);
    var JSONGame = GetJSONFromPage(GameID);
    var g = JSON.parse(JSONGame);
    g = ThrowCurrent(g);
    RefreshGameHid(g);
    HighlightSelectedFrame(g.ID, g.CurrentFrame, g.CurrentThrow);
    //UpdateCurrentThrowVal(GameID);
    //var JSONGame = GetJSONFromPage(GameID);
    //var GameIDs = GetGameIDs();
    //var getReportColumnsParams = {
    //    "JSONGame": JSONGame,
    //    "GameIDs": GameIDs
    //};
    //$.ajax({
    //    type: "POST",
    //    traditional: true,
    //    async: false,
    //    cache: false,
    //    url: '/Game/NextThrowClick',//'@Url.Action("NextThrowClick", "Game")',
    //    context: document.body,
    //    data: getReportColumnsParams,
    //    success: function (result)
    //    {
    //        //reload all of the game data on the page
    //        var returnedGame = JSON.parse(result.jsonGameReturned);
    //        RefreshGameHid(returnedGame);

    //        HighlightSelectedFrame(returnedGame.ID, returnedGame.CurrentFrame, returnedGame.CurrentThrow);

    //    },
    //    error: function (xhr)
    //    {
    //        //debugger;
    //        console.log(xhr.responseText);
    //        alert(xhr.responseText);

    //    }
    //});
}

function PreviousClickSendGame(GameID)
{
    var JSONGame = GetJSONFromPage(GameID);
    var g = JSON.parse(JSONGame);
    g = CalculatePrevious(g);
    RefreshGameHid(g);
    HighlightSelectedFrame(g.ID, g.CurrentFrame, g.CurrentThrow);
    //var JSONGame = GetJSONFromPage(GameID);
    //var GameIDs = GetGameIDs();
    //var getReportColumnsParams = {
    //    "JSONGame": JSONGame,
    //    "GameIDs": GameIDs
    //};
    //$.ajax({
    //    type: "POST",
    //    traditional: true,
    //    async: false,
    //    cache: false,
    //    url: '/Game/PreviousThrowClick',//'@Url.Action("PreviousThrow", "Game")',
    //    context: document.body,
    //    data: getReportColumnsParams,
    //    success: function (result)
    //    {
    //        var returnedGame = JSON.parse(result.jsonGameReturned);
    //        RefreshGameHid(returnedGame);

    //        HighlightSelectedFrame(returnedGame.ID, returnedGame.CurrentFrame, returnedGame.CurrentThrow);
    //    },
    //    error: function (xhr)
    //    {
    //        //debugger;
    //        console.log(xhr.responseText);
    //        alert(xhr.responseText);

    //    }
    //});
}

function SaveGameClick(GameID)
{
    var JSONGame = GetJSONFromPage(GameID);
    var getReportColumnsParams = {
        "JSONGame": JSONGame,
        "GameID": GameID
    };
    $.ajax({
        type: "POST",
        traditional: true,
        async: false,
        cache: false,
        url: '/Game/SaveGameClick',//'@Url.Action("PreviousThrow", "Game")',
        context: document.body,
        data: getReportColumnsParams,
        success: function (result)
        {
            var returnedGame = JSON.parse(result.jsonGameReturned);
            if (result.redirect)
            {
                //Redirect to the Edit page after a new game has been created
                window.location = "/Game/Edit/" + returnedGame.ID;
                return;
            }

            RefreshGameHid(returnedGame);

            HighlightSelectedFrame(returnedGame.ID, returnedGame.CurrentFrame, returnedGame.CurrentThrow);

            $("#SavedAlert").fadeTo(4000, 500).slideDown(500, function ()
            {
                $("#SavedAlert").slideUp(500);
            });
        },
        error: function (xhr)
        {
            //debugger;
            console.log(xhr.responseText);
            alert(xhr.responseText);

        }
    });
}

function SaveAllClick()
{
    var GameIDs = GetGameIDs();
    for (var i = 0; i < GameIDs.length; i++)
    {
        SaveGameClick(GameIDs[i]);
    }
}



//--------------------
//utils
//--------------------
function GetGameIDs()
{
    var result = [];
    $(".hidGameID").each(function (i, obj)
    {
        result.push($(this).val());
    });

    return (result);
}

function GetThrowNum(CurrentThrow)
{
    if (CurrentThrow === 21)
    {
        return (3);
    }
    else if (CurrentThrow % 2 === 0)
    {
        return (2);
    }
    else if (CurrentThrow % 2 === 1)
    {
        return (1);
    }
}








//--------------------
//Scoring
//--------------------

function ScoreGame(g)
{
    var score = 0;

    for (var i = 0; i < 10; i++)
    {
        g.Frames[i].FrameScore = parseInt(g.Frames[i].FrameScore);

        if (g.Frames[i].ThrowOneScore == 10)
        {
            g.Frames[i].FrameScore = StrikeBonus(g.Frames, i);
        }
        else if (g.Frames[i].ThrowOneScore + g.Frames[i].ThrowTwoScore == 10)
        {
            g.Frames[i].FrameScore = SpareBonus(g.Frames, i);
        }
        else
        {
            g.Frames[i].FrameScore = parseInt(g.Frames[i].ThrowOneScore) + parseInt(g.Frames[i].ThrowTwoScore);
        }

        g.Frames[i].ThrowOneScore = parseInt(g.Frames[i].ThrowOneScore);
        g.Frames[i].ThrowTwoScore = parseInt(g.Frames[i].ThrowTwoScore);

        if ((g.Frames[i].FrameNum == 10) && (g.Frames[i].ThrowOneScore + g.Frames[i].ThrowTwoScore < 10))
        {
            g.Frames[i].ThrowThreeScore = 0;
        }

        if (g.Frames[i].ThrowOneScore == 10 && g.Frames[i].FrameNum != 10)
        {
            g.Frames[i].ThrowTwoScore = 0;
        }

        score += g.Frames[i].FrameScore;
        g.Frames[i].FrameTotal = score;
    }

    g.Score = score;
    return (g);
}

function StrikeBonus(frameslist, frameindex)
{
    if (frameindex == 8)
    {
        frameslist[9].ThrowOneScore = parseInt(frameslist[9].ThrowOneScore);
        frameslist[9].ThrowTwoScore = parseInt(frameslist[9].ThrowTwoScore);

        return (10 + frameslist[9].ThrowOneScore + frameslist[9].ThrowTwoScore);
    }
    else if (frameindex == 9)
    {
        frameslist[frameindex].ThrowOneScore = parseInt(frameslist[frameindex].ThrowOneScore);
        frameslist[frameindex].ThrowTwoScore = parseInt(frameslist[frameindex].ThrowTwoScore);
        frameslist[frameindex].ThrowThreeScore = parseInt(frameslist[frameindex].ThrowThreeScore);

        return (frameslist[frameindex].ThrowOneScore + frameslist[frameindex].ThrowTwoScore + frameslist[frameindex].ThrowThreeScore);
    }
    else
    {
        frameslist[frameindex + 1].ThrowOneScore = parseInt(frameslist[frameindex + 1].ThrowOneScore);
        
        if (frameslist[frameindex + 1].ThrowOneScore == 10)
        {
            frameslist[frameindex + 2].ThrowOneScore = parseInt(frameslist[frameindex + 2].ThrowOneScore);

            return (10 + frameslist[frameindex + 1].ThrowOneScore + frameslist[frameindex + 2].ThrowOneScore);
        }
        else
        {
            frameslist[frameindex + 1].ThrowTwoScore = parseInt(frameslist[frameindex + 1].ThrowTwoScore);

            return (10 + frameslist[frameindex + 1].ThrowOneScore + frameslist[frameindex + 1].ThrowTwoScore);
        }
    }
}

function SpareBonus(frameslist, frameindex)
{
    if (frameslist[frameindex].FrameNum == 10)
    {
        frameslist[frameindex].ThrowThreeScore = parseInt(frameslist[frameindex].ThrowThreeScore);
        return (10 + frameslist[frameindex].ThrowThreeScore);
    }
    else
    {
        frameslist[frameindex].ThrowOneScore = parseInt(frameslist[frameindex].ThrowOneScore);
        return (10 + frameslist[frameindex].ThrowOneScore);
    }

}


//------------------------------------

function ThrowCurrent(g)
{
    var ThrowOneScore = g.Frames[g.CurrentFrame - 1].ThrowOneScore;

    g = CalculateNext(g, ThrowOneScore);

    g = ScoreGame(g);

    if (g.ScoreUpToFrame < g.CurrentFrame)
    {
        g.ScoreUpToFrame = g.CurrentFrame;
    }
    return (g);
}

function CalculateNext(g, ThrowOneScore)
{
    ThrowOneScore = parseInt(ThrowOneScore);

    var ThrowNum = GetThrowNum(parseInt(g.CurrentThrow));

    if (ThrowNum == 1)
    {
        if (g.CurrentFrame != 10 && ThrowOneScore == 10)
        {
            g.CurrentFrame++;
        }
    }
    else if (ThrowNum == 2)
    {
        if (g.CurrentFrame != 10)
        {
            g.CurrentFrame++;
        }
    }
    g = CalculateCurrentThrow(g, ThrowOneScore, true);
    return (g);
}

function CalculatePrevious(g)
{
    g.CurrentFrame = parseInt(g.CurrentFrame);

    var ThrowNum = GetThrowNum(parseInt(g.CurrentThrow));
    if (ThrowNum == 1 && g.CurrentFrame != 1)
    {
        g.CurrentFrame--;
    }

    g = CalculateCurrentThrow(g, 0, false);
    return (g);
}

function CalculateCurrentThrow(g, ThrowOneScore, forward)
{
    g.CurrentThrow = parseInt(g.CurrentThrow);
    g.CurrentFrame = parseInt(g.CurrentFrame);
    
    var ThrowNum = GetThrowNum(parseInt(g.CurrentThrow));

    if (g.CurrentFrame < 10)
    {
        if (ThrowNum == 1 && ThrowOneScore != 10)
        {
            g.CurrentThrow = (g.CurrentFrame * 2);
        }
        else
        {
            g.CurrentThrow = (g.CurrentFrame * 2) - 1;
        }
    }
    else
    {
        if (forward)
        {
            if (g.CurrentThrow == 19)
            {
                g.CurrentThrow++;
            }
            else if (g.CurrentThrow == 20)
            {
                if (ThrowOneScore + parseInt(g.Frames[g.CurrentFrame - 1].ThrowTwoScore) >= 10)
                {
                    g.CurrentThrow++;
                }
            }
            else if (g.CurrentThrow == 21)
            { }
            else
            {
                g.CurrentThrow = 19;
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
