// Write your JavaScript code.


///Getting GameData from the page (labels) to be sent to Controller
///Returns JSON string of the Game
function GetGameData()
{
    var frameDetails = {};
    var game = {};
    game.frames = [];
    game.ID = '@Model.ID';
    game.CreatedDate = $("#game_CreatedDate").val();
    game.ModifiedDate = $("#game_ModifiedDate").val();
    game.CurrentFrame = $("#game_CurrentFrame").val();
    game.CurrentThrow = $("#game_CurrentThrow").val();
    game.UserID = '@Model.UserID';

    //SetPrevious(game);
    ClearHighlight(game.CurrentFrame);

    for (var i = 1; i <= 10; i++)
    {
        var frameDetails = {};
        //frameDetails["FrameNum"] = i; //maybe not edit framenum
        frameDetails["GameID"] = '@Model.ID';
        frameDetails["ID"] = $("#" + i + "_FrameID").val();

        frameDetails["FrameScore"] = $("#" + i + "_FrameScore").val(); //Hidden field value. TODO: Confirm this
        frameDetails["FrameTotal"] = $("#" + i + "_FrameTotal").text().trim();

        frameDetails["FrameNum"] = $("#" + i + "_FrameNum").val();

        //---Throw Data---
        //TODO: This needs to know if it is first throw to the second throw
        // Currently this is setting the second throw score to 10
        // It needs to be based on the first throw, if it is less than 10
        var throwOne_data = GetThrowPins(i, 1);
        var throwTwo_data = GetThrowPins(i, 2);
        frameDetails["ThrowOneScore"] = throwOne_data.score;//$("#" + i + "_FirstThrow").text().trim();
        frameDetails["ThrowTwoScore"] = throwTwo_data.score - throwOne_data.score;//$("#" + i + "_SecondThrow").text().trim();
        frameDetails["ThrowOnePins"] = throwOne_data.missed;
        frameDetails["ThrowTwoPins"] = throwTwo_data.missed;

        /*
        //CurrentThrow is WHAT IS BEING THROWN NOW. After it returns from controller it will be higher
        var ThrowNum = GetThrowNum(game.CurrentThrow);
        if (ThrowNum == 1)
        {
            //First throw going to 2 (or next 1 if frame 1-9)
            frameDetails["ThrowTwoScore"] = 10 - throwOne_data.score;
            frameDetails["ThrowTwoPins"] = MISSED_ALL - throwOne_data.missed;
        }
        else if (ThrowNum == 2)
        {
            //calculate throwtwo score based on throw one
            frameDetails["ThrowTwoScore"] = (throwTwo_data.score - throwOne_data.score);
        }
        else if (ThrowNum == 3)
        {
            //This is only called when: 10th frame, 3rd throw, previous clicked.

        }
        */


        //---Throw Data---


        if (i == 10)
        {
            if (throwOne_data.score == 10)
            {
                frameDetails["ThrowTwoScore"] = throwTwo_data.score;
            }
            var throwThree_data = GetThrowPins(i, 3);
            frameDetails["ThrowThreePins"] = throwThree_data.missed;

            //throw one AND two strike OR throw one and two spare
            if ((throwOne_data.score + throwTwo_data.score == 20) || (throwOne_data.score + (throwTwo_data.score - throwOne_data.score) == 10))
            {
                //Fresh third throw
                frameDetails["ThrowThreeScore"] = throwThree_data.score;
            }
            else if (throwOne_data.score == 10) //throw one strike, throw two NOT strike
            {
                frameDetails["ThrowThreeScore"] = 10 - throwTwo_data.score;
            }

        }

        //TODO: Hidden fields for the rest of the fields here? i.e. framescore, gameID, etc.

        game.frames.push(frameDetails)
    }
    return JSON.stringify(game);
}



function TestThisFile()
{
    alert("Yepp");
}

function NextThrow(GameID)
{
    alert("GameID passed: " + GameID);
}