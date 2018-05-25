****ASP.NET Core MVC Bowling App****  
https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/?view=aspnetcore-2.0



**Secrets**  
Storing secret values in the application the safe way:  
	right-click project > Manage User Secrets  
	This is our configuration info that won't get put into source control



**Look into using a TagHelper for displaying frames**  
https://mva.microsoft.com/en-US/training-courses/aspnet-core-intermediate-18154?l=QiFcbpbeE_811787171  
https://docs.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/authoring?view=aspnetcore-2.1



**In ASP CORE, use VIEW COMPONENTS instead of partial views**
https://docs.microsoft.com/en-us/aspnet/core/mvc/views/view-components?view=aspnetcore-2.0


Process overview for the javascript design

so what needs to happen for a request to edit a single game


Load the edit game page
	pass in either 1 Model.Game or a list




game/frame tag helpers

    <table>
	    <tr>
		    <td>[throw one]
			    <label>throw one formatted text</label>
			    <hidden>throw one score</hidden>
		    </td>
		    <td>throw two</td>
	    </tr>
	    <tr>
		    <td>frame score</td>
	    </tr>
	    <tr>
		    <td>frame pins</td>
	    </tr>
    </table>




What if I had ONE set of checkboxes for the pins
that would make it look a LOT cleaner..
but then I would want to view the missed pins at a glance, without it being selected..






How the games are getting loaded for edit currently: (just as a reminder)

	Game/Edit.cshtml
		loads EditGame ViewComponent [/ViewComponenets/EditGameViewComponent.cs] for each game
			loads game from DB and returns View(game) [Views/shared/Components/EditGame/Default.cshtml]
				loads the game and uses tag helpers

		.js is located in site.js






I need to figure out this UpdateGame stuff
Currently, I have trying to mash together the Game from the database in with the one from the Page
this isn't working because it is overriding correct values with incorrect ones

I can probably keep it the way it is (page game not having every field)
but I still need to do this mashing together before an update

*I shouldn't need to call this update on next or previous click*









soooo, couple issues, trying to finish up scoring

the second ball on all but the 10th frame is not saving
	this is leading to the score being adjusted when it sholdn't

on page load, the pins aren't being loaded



todo:
make sure scoreupto is being passed through JSON to the game
	done; it was, just not being updated in js (refresh)



TODO: (features)
	css rule for formatting the frames text
		(no css rule exists for 'contains')
	warn the user if they have made changes and haven't saved them before leaving the page




Secrets are only used for development, to prevent passwords being put into source control
For production, there is Azure Key Vault configurations
	https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-2.1&tabs=aspnetcore2x

Easy to follow blog about setting up secrets:
https://blogs.msdn.microsoft.com/mihansen/2017/09/10/managing-secrets-in-net-core-2-0-apps/


really good answer about getting lists into a view
https://stackoverflow.com/questions/34624034/select-tag-helper-in-asp-net-core-mvc?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa



goals for today (5-4):

Confirm creation of series and leagues is done properly; done
series list showing league name instead of ID; done
clean up lists and order them by created date; done?
finish up game scoring, make sure there are no bugs
clean up authentication; comment out things the user shouldn't see
???
publish v1 to azure



I need to add locations to the app
	any user can add a 'location'
	leagues will be organized per location
		"Grand Raapids Area" - where all leagues in that location are held
	this will be to minimize what leagues are shown as an option

maybe add a 'view league' page/s
	show weekly stats/summaries
	keep track of highscores


ok ok ok, some changes
when creating a series, the league list needs to be filtered by WHAT LEAGUES THE USER IS IN
needs to be a 'join league' page (manage league too? see it's users?)
	if we want to be really thourough, have a user request to join a league
	and have the creator of the league approve or deny it.

more permissions rules
only the person who has created the location can delete the location
only the person who has created the league can delete the league
	maybe have a limit on these? if it has existed for more than 10 days, don't allow delete ever


toolbar
	make login panel a dropdown

	other dropdowns possibly
	Manage Leagues > 
		locations
		leagues
		view leagues?
	My Stuff > 
		Games
		Series
		Leagues










so, dependency injection is kinda neat  
	https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1#registering-services

Instead of passing the datacontext directly to each controller,
you could instead pass in an interface of the type you are dealing with
and have that interface implement whatever dataaccess you need
with the interface getting the context instead.
(yet another layer of abstraction just to organize things...)

so it's   startup > dbcontext > ILeague > LeaguesController  
insead of startup > dbcontext > LeaguesController





if (ScoreUpToFrame < currentFrame)
	format 0s as ' '
else
	format 0s as '-'




TODO: The Create new game redirecting to the Edit page is a little jarring

hidden value is getting set when refresh game is getting called on current frame before it is getting thrown



{

    So I don't like using the ViewModels everywhere
    it seems like bad design that I have 3 different models for 1 db object

    my idea now is to have EVERYTHING in my main model
    would this have any issues?...


    so it's one to many vs. many to one

    a Game has a list of Frames, the frame has a GameID
	    Game 1 --> * Frames

    a League has a LocationID, locations have all possible locations
	    League 1 <-- * Locations


    I did this ^
    removed the viewmodels and using [notmapped] fields
}





5-14  
{

	deleting a series, maybe have it warn the user if it still has games?
	delete the games first before deleting the series?

	the game list should have some series/league info if it is available
		with a link to the series

	somehow enable paging on all the lists


}

5-15  
{

	this tutorial is the one that ended up working
	https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-dotnetcore-sqldb

	REMEMBER TO TURN OFF DEVELOPER ERRORS IN STARTUP AT SOME POINT
}



Production Update checklist  
{

	Any database changes, run: dotnet migrations add [name]
	Any .js or .css changes, run: dotnet bundle


	push to source control default branch
}

5-21  
{

	happy birthday

	so leagues and series have some work to do
		leagues need a 'league sheet' with scores etc.

	series editing:
	for one user editing multiple users games: (also, permission for this user to do that??)
	distinction between 'edit my own series' vs 'edit multiple users games for a series'
	an addition to:
		series?
		league?
		new entity: team?

	We should probably just go with the team idea
	Team Captain will have permission to create/edit the teams games
	

    {
        TEAM:
        TeamID
        LeagueID
        TeamName
        
        TEAM TO USER:
        TeamID
        UserID


    }

    I want to be able to get: a specific users games as a part of one team
    select * from Games g, TeamToUser tu, Team t
    where g.UserID == tu.UserID
    and t.TeamID == tu.TeamID
    and g.UserId == [myuser]
    



    should I just change Series and Games to have a TeamID instead of a LeagueID?
    or maybe have both?
    and g.TeamID == [myteam]

    when creating a series and picking a league, it should automatically pick the team i am a part of from that league


    select * from series s, games g
    where s.SeriesID = g.ID
    and s.TeamID = [myteam]


    after some thought, here is a design that might work
    {
    	TEAM:
    	ID
    	LeagueID

    	Series:
    	-Remove LeagueID
    	-Add TeamID

    	TeamToUser: (list of users on a team)
    	ID
    	UserID
    	TeamID
    }



    After these changes, I would want to be able to filter the Series list by team / league
    	and view the games! (summary at least)

    

left the day at:
messing with migrations, will need to get this working for any prod updates
trying to initialize the db with valid data and a new user

}


5-23
{

	going to attempt to put a bunch of the create screens into partial views
	doing this will allow inline creation of things
	current process is to: Create location > create league > create team > create series...
	that's way too many steps just to get started.

	so in creating a new series, we will have a partial of all those things?
	series will need to have virtual objects of all these things too then..


	actually, let's cut out the series, series is it's own thing
	location > league > team : should all be combined

	hmmmmmmmmmmm, partial views or tag helpers....

	really kind of annoyed with this project at this point
	most of the work is being put into making the pages look pretty or function properly
	I really need a non webdev project to work on


	I just need to rethink the combined creation pages a bunch more
	need to figure out the process and make that air tight before I try to code it
	Too often I just start coding without thinking through the process
	this leads to a lot of wasted time
	maybe come up with a process that i can do every time i want to start on a new feature
	....some sort of life cycle, for software specifically perhaps.. in like, a waterfall shape..
	blah.
}

5-24
{

    what i did now makes a whole lot more sense and is a lot simpler
    i simply added the option to create a Location when creating a League
    It doesn't need to be any more than that
    everything else is their own entitie and doesn't make sense to combine them so much

    ANYWAY
    let's move on. Teams.
    Creation of a Team should include tagging users to that TEam
    
    Maybe I should create a 'fake' user that someone can add (if those Users don't exist)
    Then someone (ie. a team captain, or user of the site) can log the whole teams scores

    alright, so fake users.
    fakes users will have a parent

    FakeUser
    {
    	ID
    	ParentUserID
    	UserName

    }

    FakeUsername will not block actual users names
    new users should be able to 'connect' themselves with fakeusers somehow?
    fakeusers will have their own games, series, etc.
    fakeusers should not be able to really do things, only its parent user
    when a parentuser creates a new game/series, will they have to establish whos game it is?
    or should fakeusers be only used as a part of a team?
    then when there is a league night, the team captain creates the session
    	which creates a series for all users a part of the team

    is this feature really worth it? who would actually be using this?
    right?
    this site is about YOUR OWN SCORES, and competing against other users
    not necessarily logging EVERYONES scores..
    for a tournament feature, i would want everyone who is competing to be a user
    for teams, it makes sense that they are users too.

    ended up addinga usersummary page
    
}

5-25
{

	trying to think of other features I could add that would improve on what I already have
	I want to keep it simple, but think of features I would want out of something like this
	remember, this is MY application. At this point, I should only add features that I want.

	performance option without moving everything to the client side:
		only score the game when the game is saved.
		on next click, only update the frames throws
		when the game is saved, update the framescores

	
}



