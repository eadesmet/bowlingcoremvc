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
    var frameDetails = {};
    var game = {};
    game.frames = [];
    game.ID = GameID;
    //game.CreatedDate = $("#game_CreatedDate").val();
    //game.ModifiedDate = $("#game_ModifiedDate").val();
    game.CurrentFrame = $("#" + GameID + "_CurrentFrame").val();
    game.CurrentThrow = $("#" + GameID + "_CurrentThrow").val();
    game.ScoreUpToFrame = $("#" + GameID + "_ScoreUpToFrame").val();
    //game.UserID = '@Model.UserID';

    //SetPrevious(game);
    ClearHighlight(GameID, game.CurrentFrame);

    for (var i = 1; i <= 10; i++)
    {
        var frameDetails = {};

        var prefixID = "#" + GameID + "_" + i + "_";

        frameDetails["GameID"] = GameID;
        frameDetails["ID"] = $(prefixID + "FrameID").val();

        frameDetails["FrameScore"] = $(prefixID + "hidFrameScore").val();
        frameDetails["FrameTotal"] = $(prefixID + "hidFrameTotal").val();
        frameDetails["FrameNum"] = $(prefixID + "hidFrameNum").val();

        //Update the hidden values for the score/pins of the current throw
        if (i == game.CurrentFrame)
        {
            var ThrowNum = GetThrowNum(game.currentThrow);
            var CurrentThrowData = GetThrowPins(GameID, game.CurrentFrame, ThrowNum);
            //CONFIRM THESE .VAL ARE GETTING SET (might be overwritten in refresh game? make sure framedetails is getting updated)
            $(prefixID + ThrowNum + "_hidFrame").val(CurrentThrowData.score);
            $(prefixID + ThrowNum + "_hidPins").val(CurrentThrowData.missed);
        }

        frameDetails["ThrowOneScore"] = $(prefixID + "1_hidFrame").val();
        frameDetails["ThrowTwoScore"] = $(prefixID + "2_hidFrame").val();

        frameDetails["ThrowOnePins"] = $(prefixID + "1_hidPins").val();
        frameDetails["ThrowTwoPins"] = $(prefixID + "2_hidPins").val();

        

        if (i == 10)
        {
            frameDetails["ThrowThreeScore"] = $(prefixID + "3_hidFrame").val();
            frameDetails["ThrowThreePins"] = $(prefixID + "3_hidPins").val();
        }

        game.frames.push(frameDetails)
    }
    return JSON.stringify(game);
}

///Goes through the checkboxes and gets the Integer of missed pins
function GetThrowPins(GameID, frameNum, throwNum)
{
    var missed_pins = 0;
    var throw_score = 10;

    var one =   $("#" + GameID + "-Missed_1").is(':checked');
    var two =   $("#" + GameID + "-Missed_2").is(':checked');
    var three = $("#" + GameID + "-Missed_3").is(':checked');
    var four =  $("#" + GameID + "-Missed_4").is(':checked');
    var five =  $("#" + GameID + "-Missed_5").is(':checked');
    var six =   $("#" + GameID + "-Missed_6").is(':checked');
    var seven = $("#" + GameID + "-Missed_7").is(':checked');
    var eight = $("#" + GameID + "-Missed_8").is(':checked');
    var nine =  $("#" + GameID + "-Missed_9").is(':checked');
    var ten =   $("#" + GameID + "-Missed_10").is(':checked');


    if (one)   { missed_pins = missed_pins + MISSED_1; throw_score = throw_score - 1; };
    if (two)   { missed_pins = missed_pins + MISSED_2; throw_score = throw_score - 1; };
    if (three) { missed_pins = missed_pins + MISSED_3; throw_score = throw_score - 1; };
    if (four)  { missed_pins = missed_pins + MISSED_4; throw_score = throw_score - 1; };
    if (five)  { missed_pins = missed_pins + MISSED_5; throw_score = throw_score - 1; };
    if (six)   { missed_pins = missed_pins + MISSED_6; throw_score = throw_score - 1; };
    if (seven) { missed_pins = missed_pins + MISSED_7; throw_score = throw_score - 1; };
    if (eight) { missed_pins = missed_pins + MISSED_8; throw_score = throw_score - 1; };
    if (nine)  { missed_pins = missed_pins + MISSED_9; throw_score = throw_score - 1; };
    if (ten)   { missed_pins = missed_pins + MISSED_10; throw_score = throw_score - 1; };

    return { missed: missed_pins, score: throw_score };
}

//------------------Refresh functions------------------
///This only updates the hidden fields for the game passed in
function RefreshGameHid(g)
{
    $("#" + g.ID + "_CurrentFrame").val(g.CurrentFrame);
    $("#" + g.ID + "_CurrentThrow").val(g.CurrentThrow);

    for (var i = 1; i <= 10; i++)
    {
        $("#" + g.ID + "_" + i + "_hidFrameScore").val(g.Frames[i - 1].FrameScore);
        $("#" + g.ID + "_" + i + "_hidFrameTotal").val(g.Frames[i - 1].FrameTotal);
        $("#" + g.ID + "_" + i + "_1_hidFrame")   .val(g.Frames[i - 1].ThrowOneScore);
        $("#" + g.ID + "_" + i + "_2_hidFrame").val(g.Frames[i - 1].ThrowTwoScore);
        $("#" + g.ID + "_" + i + "_1_hidPins").val(g.Frames[i - 1].ThrowOnePins);
        $("#" + g.ID + "_" + i + "_2_hidPins").val(g.Frames[i - 1].ThrowTwoPins);

        if (i == 10)
        {
            $("#" + g.ID + "_" + i + "_3_hidFrame").val(g.Frames[i - 1].ThrowThreeScore);
            $("#" + g.ID + "_" + i + "_3_hidPins").val(g.Frames[i - 1].ThrowThreePins);
        }

        SetLabelsFromHid(g.ID, i);

        // : MissedPinsINT Bitwise & MISSED_1 : (In javascript, this should return 1 if it was missed)
        //$("#" + i + "_1_MissedPinOne").prop("checked", (g.Frames[i - 1].ThrowOnePins & MISSED_1) == MISSED_1);
        
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
    //NOTE: This will be null if CurrentThrow == 1, but we don't use it then anyway
    var PrevMissedPins = $("#" + GameID + "_" + FrameNum + "_" + (GetThrowNum(CurrentThrow-1)) + "_hidPins").val();

    //reset (enable all)
    EnableAllMissedPins(GameID);

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
        if (PrevMissedPins == 0)
        {
            //all enabled, set saved missed
            SetMissedPinsChecked(GameID, MissedPins)
        }
        else
        {
            //previous missed enabled, set saved missed
            SetMissedPinsEnabled(GameID, PrevMissedPins);
            SetMissedPinsChecked(GameID, MissedPins);
        }
    }




    if (0)
    {
        //TODO: Check previous throw and other logic here

        //If first throw on any frame, enable all and set checked
        //If second throw on frames 1-9, disable pins from previous throw, set missed pins checked
        //If 10th second throw, if first throw 10, all enabled and set checked
        //if 10th third throw, if second throw 10 OR first+second == 10, all enabled and set checked

        //remember, we ONLY need to do the CURRENT throw

        //var ThrowNum = GetThrowNum(CurrentThrow);
        //var previousThrowMissed = $("#" + GameID + "_" + CurrentFrame + "_" + CurrentThrow - 1 + "_hidPins").val();

        //if (ThrowNum == 1)
        //{
        //    //set checked (if there were any)
        //    SetMissedPinsChecked(GameID);

        //}
        //else if (ThrowNum == 2 && FrameNum != 10)
        //{
        //    //frames 1-9 second throw
        //    //If we get here (thrownum == 2) then there WERE missed pins on the previous throw


        //    if (ScoreUpToFrame < FrameNum)
        //    {
        //        //if we haven't scored up to this point yet, set the second throw fresh from the first throw
        //        SetMissedPinsChecked(previousThrowMissed);
        //    }
        //    else
        //    {
        //        //If we have scored up  to this point before, set them to what has been saved for the second throw
        //        SetMissedPinsChecked(MissedPins);
        //    }

        //}

        //if (FrameNum == 10 && ThrowNum == 2)
        //{
        //    //10th frame, second throw. check if first was strike
        //    if (previousThrowMissed == 0)
        //    {
        //        //10th frame first throw was a strike, enable all
        //    }
        //    else
        //    {
        //        //10th frame first throw was not a strike, enable missed
        //    }
        //}
        //else if (FrameNum == 10 && ThrowNum == 3)
        //{
        //    //10th frame, third throw. check if second ball 10 OR first+second == 10
        //    if (previousThrowMissed > 0)
        //    {
        //        //if second throw missed any, and ThrowNum is considered 3, then this is for a spare

        //    }
        //    else
        //    {
        //        //enable all
        //    }

        //}
    }
}

function CheckAndSetMissedPins(GameID, FrameNum, MissedPins, ScoreUpToFrame)
{
    //var previousThrowMissed = $("#" + GameID + "_" + CurrentFrame + "_" + CurrentThrow - 1 + "_hidPins").val();
    //if (ScoreUpToFrame > FrameNum)
    //{
    //    //set to what our value is
    //    SetMissedPinsChecked(GameID, MissedPins)
    //}
    //else
    //{
        
    //}
}

function SetMissedPinsChecked(GameID, MissedPins)
{
    $("#" + GameID + "-Missed_1").prop("checked", (MissedPins & MISSED_1) == MISSED_1);
    $("#" + GameID + "-Missed_2").prop("checked", (MissedPins & MISSED_2) == MISSED_2);
    $("#" + GameID + "-Missed_3").prop("checked", (MissedPins & MISSED_3) == MISSED_3);
    $("#" + GameID + "-Missed_4").prop("checked", (MissedPins & MISSED_4) == MISSED_4);
    $("#" + GameID + "-Missed_5").prop("checked", (MissedPins & MISSED_5) == MISSED_5);
    $("#" + GameID + "-Missed_6").prop("checked", (MissedPins & MISSED_6) == MISSED_6);
    $("#" + GameID + "-Missed_7").prop("checked", (MissedPins & MISSED_7) == MISSED_7);
    $("#" + GameID + "-Missed_8").prop("checked", (MissedPins & MISSED_8) == MISSED_8);
    $("#" + GameID + "-Missed_9").prop("checked", (MissedPins & MISSED_9) == MISSED_9);
    $("#" + GameID + "-Missed_10").prop("checked", (MissedPins & MISSED_10) == MISSED_10);
}

function SetMissedPinsEnabled(GameID, MissedPins)
{
    //MissedPins here should be the previous frames missed pins (also, disabled false == enabled)
    $("#" + GameID + "-Missed_1").prop("disabled", (MissedPins & MISSED_1) != MISSED_1);
    $("#" + GameID + "-Missed_2").prop("disabled", (MissedPins & MISSED_2) != MISSED_2);
    $("#" + GameID + "-Missed_3").prop("disabled", (MissedPins & MISSED_3) != MISSED_3);
    $("#" + GameID + "-Missed_4").prop("disabled", (MissedPins & MISSED_4) != MISSED_4);
    $("#" + GameID + "-Missed_5").prop("disabled", (MissedPins & MISSED_5) != MISSED_5);
    $("#" + GameID + "-Missed_6").prop("disabled", (MissedPins & MISSED_6) != MISSED_6);
    $("#" + GameID + "-Missed_7").prop("disabled", (MissedPins & MISSED_7) != MISSED_7);
    $("#" + GameID + "-Missed_8").prop("disabled", (MissedPins & MISSED_8) != MISSED_8);
    $("#" + GameID + "-Missed_9").prop("disabled", (MissedPins & MISSED_9) != MISSED_9);
    $("#" + GameID + "-Missed_10").prop("disabled", (MissedPins & MISSED_10) != MISSED_10);
}

function EnableAllMissedPins(GameID)
{
    $("#" + GameID + "-Missed_1").prop("disabled",  false);
    $("#" + GameID + "-Missed_2").prop("disabled",  false);
    $("#" + GameID + "-Missed_3").prop("disabled",  false);
    $("#" + GameID + "-Missed_4").prop("disabled",  false);
    $("#" + GameID + "-Missed_5").prop("disabled",  false);
    $("#" + GameID + "-Missed_6").prop("disabled",  false);
    $("#" + GameID + "-Missed_7").prop("disabled",  false);
    $("#" + GameID + "-Missed_8").prop("disabled",  false);
    $("#" + GameID + "-Missed_9").prop("disabled",  false);
    $("#" + GameID + "-Missed_10").prop("disabled", false);
}


//-----------------------Setting Labels---------------------
function SetLabelsFromHid(GameID, FrameNum)
{
    EmptyFrameLabels(GameID, FrameNum)

    $("#" + GameID + "_" + FrameNum + "_lblFrameTotal").append($("#" + GameID + "_" + FrameNum + "_hidFrameTotal").val());
    $("#" + GameID + "_" + FrameNum + "_1_lblFrame").append($("#" + GameID + "_" + FrameNum + "_1_hidFrame").val());
    $("#" + GameID + "_" + FrameNum + "_2_lblFrame").append($("#" + GameID + "_" + FrameNum + "_2_hidFrame").val());

    if (FrameNum == 10)
    {
        $("#" + GameID + "_" + FrameNum + "_3_lblFrame").append($("#" + GameID + "_" + FrameNum + "_3_hidFrame").val());
    }

}

function EmptyFrameLabels(GameID, FrameNum)
{
    $("#" + GameID + "_" + FrameNum + "_lblFrameTotal").empty();
    $("#" + GameID + "_" + FrameNum + "_1_lblFrame").empty();
    $("#" + GameID + "_" + FrameNum + "_2_lblFrame").empty();
    if (FrameNum == 10)
    {
        $("#" + GameID + "_" + FrameNum + "_3_lblFrame").empty();
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

    if (FrameNum == 10)
    {
        $(prefixID + "3_tdFrame").css("background-color", "White");
    }
}

function HighlightSelectedFrame(GameID, FrameNum, CurrentThrow)
{
    //NOTE: Might not need CurrentThrow, we can get it ourselves here
    var prefixID = "#" + GameID + "_" + FrameNum + "_";

    $(prefixID + "1_tdFrame").css("background-color", "Aquamarine");
    $(prefixID + "2_tdFrame").css("background-color", "Aquamarine");
    $(prefixID + "tdFrameScore").css("background-color", "Aquamarine");
    if (FrameNum == 10)
    {
        $(prefixID + "3_tdFrame").css("background-color", "Aquamarine");
        if (CurrentThrow == 21)
        {
            $(prefixID + "3_tdFrame").css("background-color", "Plum");
        }
        else
        {
            var fThrow = CurrentThrow % 2;
            if (fThrow == 0) { fThrow = 2; }
            if (fThrow == 1)
            {
                $(prefixID + "1_tdFrame").css("background-color", "Plum");
            }
            else if (fThrow == 2)
            {
                $(prefixID + "2_tdFrame").css("background-color", "Plum");
            }
        }
    }
    else
    {
        var fThrow = CurrentThrow % 2;
        if (fThrow == 0) { fThrow = 2; }
        if (fThrow == 1)
        {
            $(prefixID + "1_tdFrame").css("background-color", "Plum");
        }
        else if (fThrow == 2)
        {
            $(prefixID + "2_tdFrame").css("background-color", "Plum");
        }
    }
}


//Document Load
$(document).ready(function ()
{
    var GameIDs = GetGameIDs();
    for (var i = 0; i < GameIDs.length; i++)
    {
        //look up the current game / frame
        var CurrentFrame = $("#" + GameIDs[i] + "_CurrentFrame").val();
        var CurrentThrow = $("#" + GameIDs[i] + "_CurrentThrow").val();
        HighlightSelectedFrame(GameIDs[i], CurrentFrame, CurrentThrow);
    }
});


//---------------------------------------
//----- AJAX CALLS ----------------------
//---------------------------------------

function NextClickSendGame(GameID)
{
    var JSONGame = GetJSONFromPage(GameID);
    var GameIDs = GetGameIDs();
    var getReportColumnsParams = {
        "JSONGame": JSONGame,
        "GameIDs" : GameIDs
    };
    $.ajax({
        type: "POST",
        traditional: true,
        async: false,
        cache: false,
        url: '/Game/NextThrowClick',//'@Url.Action("NextThrowClick", "Game")',
        context: document.body,
        data: getReportColumnsParams,
        success: function (result)
        {
            //reload all of the game data on the page
            var returnedGame = JSON.parse(result.jsonGameReturned);
            RefreshGameHid(returnedGame);

            HighlightSelectedFrame(returnedGame.ID, returnedGame.CurrentFrame, returnedGame.CurrentThrow);

        },
        error: function (xhr)
        {
            //debugger;
            console.log(xhr.responseText);
            alert(xhr.responseText);

        }
    });
}

function PreviousClickSendGame(GameID)
{
    var JSONGame = GetJSONFromPage(GameID);
    var GameIDs = GetGameIDs();
    var getReportColumnsParams = {
        "JSONGame": JSONGame,
        "GameIDs" : GameIDs
    };
    $.ajax({
        type: "POST",
        traditional: true,
        async: false,
        cache: false,
        url: '/Game/PreviousThrowClick',//'@Url.Action("PreviousThrow", "Game")',
        context: document.body,
        data: getReportColumnsParams,
        success: function (result)
        {
            var returnedGame = JSON.parse(result.jsonGameReturned);
            RefreshGameHid(returnedGame);

            HighlightSelectedFrame(returnedGame.ID, returnedGame.CurrentFrame, returnedGame.CurrentThrow);
        },
        error: function (xhr)
        {
            //debugger;
            console.log(xhr.responseText);
            alert(xhr.responseText);

        }
    });
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

function GetThrowNum(currentThrow)
{
    if (currentThrow == 21)
    {
        return (3);
    }
    else if (currentThrow % 2 == 0)
    {
        return (2);
    }
    else if (currentThrow % 2 == 1)
    {
        return (1);
    }
}