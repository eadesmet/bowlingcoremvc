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
        $("#" + g.ID + "_" + i + "_2_hidFrame")   .val(g.Frames[i - 1].ThrowTwoScore);

        if (i == 10)
        {
            $("#" + g.ID + "_" + i + "_3_hidFrame").val(g.Frames[i - 1].ThrowThreeScore);
        }

        SetLabelsFromHid(g.ID, i);

        // : MissedPinsINT Bitwise & MISSED_1 : (In javascript, this should return 1 if it was missed)
        //$("#" + i + "_1_MissedPinOne").prop("checked", (g.Frames[i - 1].ThrowOnePins & MISSED_1) == MISSED_1);
        
    }

    var CurrentThrowMissedPins = $("#" + g.ID + "_" + g.CurrentFrame + "_" + g.CurrentThrow + "_hidPins").val();

    RefreshPinsOfCurrentThrow(g.ID, CurrentThrowMissedPins, g.CurrentFrame, g.CurrentThrow);
}

function RefreshPinsOfCurrentThrow(GameID, MissedPins, FrameNum, ThrowNum)
{
    //TODO: Check previous throw and other logic here

    $("#" + GameID + "-Missed_1").prop("checked",  (MissedPins & MISSED_1) == MISSED_1);
    $("#" + GameID + "-Missed_2").prop("checked",  (MissedPins & MISSED_2) == MISSED_2);
    $("#" + GameID + "-Missed_3").prop("checked",  (MissedPins & MISSED_3) == MISSED_3);
    $("#" + GameID + "-Missed_4").prop("checked",  (MissedPins & MISSED_4) == MISSED_4);
    $("#" + GameID + "-Missed_5").prop("checked",  (MissedPins & MISSED_5) == MISSED_5);
    $("#" + GameID + "-Missed_6").prop("checked",  (MissedPins & MISSED_6) == MISSED_6);
    $("#" + GameID + "-Missed_7").prop("checked",  (MissedPins & MISSED_7) == MISSED_7);
    $("#" + GameID + "-Missed_8").prop("checked",  (MissedPins & MISSED_8) == MISSED_8);
    $("#" + GameID + "-Missed_9").prop("checked",  (MissedPins & MISSED_9) == MISSED_9);
    $("#" + GameID + "-Missed_10").prop("checked", (MissedPins & MISSED_10) == MISSED_10);
}

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
    if (frameNum == 10)
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

            //reset previous throw indicator
            //if (returnedGame.CurrentFrame != previousFrame)
            //    ClearHighlight(previousFrame);

            //previousFrame = returnedGame.CurrentFrame;
            //previousThrow = returnedGame.CurrentThrow;

            //load current throw indicator here
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