function MissedAllClick(GameID)
{
    //MissedAllClick.toggle = !MissedAllClick.toggle;
    //if (MissedAllClick.toggle)
    if ($('#' + GameID + '-btnMissedAll').val() == "true")
    {
        SetMissedPinsChecked(GameID, MISSED_ALL);
        $('#' + GameID + '-btnMissedAll').val(false);
    }
    else
    {
        SetMissedPinsChecked(GameID, MISSED_0);
        $('#' + GameID + '-btnMissedAll').val(true);
    }
}