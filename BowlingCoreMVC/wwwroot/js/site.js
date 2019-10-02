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

// Document OnLoad
$(document).ready(function () {
    //$("#sidebar").mCustomScrollbar({
    //    theme: "minimal"
    //});

    $('#sidebarCollapse').on('click', function () {
        $('#sidebar, #content').toggleClass('active');
        $('.collapse.in').toggleClass('in');
        $('a[aria-expanded=true]').attr('aria-expanded', 'false');
    });
});