
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


5-30
{

	it's been a few days since I've thought about this project
	Not sure what else I want out of it..
	I think I just need to use it for a while to learn what I want
	So that means, I need a new project.
		maybe redesign to be data oriented?
		what applications do i want to have/use in my life
		i want to move away from the web for a while if possible



	possible new projects:
		text editor (for fun, see how it works)

		[suggestions from internet]
		web server
		operating system (book)
		bandwidth monitor
		budjeting app


}

6-7
{

	moved editgame js code to it's own file, so it only loads on those pages
	thinking about moving some scoregame code to the client
		this way it won't make a round trip to the server just to throw a ball
		really it's just a handfull of functions and shouldn't take too long

	other things to do with it
		clean up the edit game page
			move the save button/s, make the arrows bigger, add title bar
		add more stats?
		add resources to learn bowling? daily tip?
		add notes section per frame
		have the game be fixed to fit the screen?
			normalize scrolling
			have each game be a 'card' view?
			have each frame be clickable to select it
			maybe have the current frame be 'zoomed' in
				showing previous and next frames a little smaller on either side
				[][][][][][][][][][]
					[]| [] |[]
					<-  \/  ->
					[        ]
		i know it's not 'needed', but maybe add 'X', '/', '-' buttons

}


6-8
{

	todo
		move all of the data access i'm doing in the controllers to a datahelper

	add a feed of recent games to the homepage?
		see bootstraps components 'media object'

}

6-12
{
	add a game details page
		show the main scoreboard
		show pins for every throw
			this will be a little tricky
			i don't really want to have 210 checkboxes on the page
			somehow generate an image? that could be interesting to figure out

	what if I combine the 'my games' and 'my series' into one?
		that would reaaaaly clean some things up and be really nice

		so instead, i could pass in a list of games that are not in a series
		and a list of series
		print both these lists in the same table and combine the series?
			or have two tables?

	change the buttons on the table to 'View' and 'Edit'
		view go to a details page, where they can then delete them if they want
		edit obvious

}

6-13
{
	not doing any of this (yet?), just thinking
	Entity component system

	Series: ID
	Game: ID, SeriesID
	Frame: ID, GameID
	FrameData: ID, FrameID, -data-

	to get a games score, get the first FrameData, then get the next 9?
}


6-18
{
	changed over the score game to js
		(not publishing it yet because league is tonight)

	having a couple issues
	I think I will need some 'GameCompleted' flag, so it doesn't clear the 10th frame when I navigate through it
	Or just try to fix some of the logic there?
		not sure where it's happening acutally
		in ScoreGame, i'm setting throwthree to 0, but changing that didn't fix it?

}

^
Just a note on the Entity component system, or data oriented design
whatever
so, it might be useful for scoring and keeping track of the frames
instead of having 'strike_bonus' and 'spare_bonus'
just have a 'score_frame(i)'
then have it handle what frame it is and calculate the score like that

if (i == 10)
{
	if (Frames[i].throwone == 10)
	{
		score strike
	}
	else
	{
		score whatever else
	}
}

would that be any better?
requires more thought.
because the strike bonus, spare bonus code would be repeated


7-12
{
	So it's been a little while.. I might have to redo a bunch of scoring logic
	The last thing I did before was move the scoring code to js
	there is now a couple 10th frame scoring bugs..


}



9-04
{
	TODO: Make the Games list (games and series page) show series games as well
		also make it the only games/series page. also add paging
		slight issue with it too, the league doesn't list on the series on one of these pages
	TODO: Series summary? Game summary x3?
	TODO: Statistics per league / per time period?

	idea - maybe have the game save automatically after being idle for x seconds?
		(on the editgame/series page)

}

9-05
{
	Ideas on fixing javascript bugs
		we should be setting all the game data into the session
		if anything changes or the page is refreshed, it's reloaded from the session
		need to ensure the session is kept up to date as possible
		could also setup an auto-save along these lines
		https://developer.mozilla.org/en-US/docs/Web/API/Window/sessionStorage

		mayybe we would want to add event listenters to everything..
		they could simply keep all the data in sync between the session and the hid values
		also when/if the hid values are updated, refresh and rescore the whole game

	ideas to bulletproof scoring
		assert that all scores are 0-10 (no negatives or greater than 10)
			if there is a score that isn't 0-10, try to re-figure out what it should have been
				this could be fun
		minimize the amount of logic flow going everywhere
		refresh/rescore everything anytime any score changes
		maybe do a second pass of scoring
			this could allow better handling of 'hiding' unconfirmed scores (scoreuptoframe)
			first pass could validate everything and score the game
			second pass could confirm the scoring and set values needed
			something like that

}

9-06
{
	Change the game table to a bootstrap flexbox
		might be easier to work with and look better on mobile
		check out bootstrap version, it might only be 4.0+
		also look at borders for these, and probably bg color too




}


10-5
{
	Making changes to allow an unsaved notification when leaving the page
	the issue is though, when creating a new game it tries to leave the page
	consider changing this to show the edit page
	maybe just create the game when clicking new game anyway.. it shouldn't be a big deal
	Should also probably combine the Leagues and Locations pages

	Ok ok ok.. I need an example here of what i'm trying to draw

	---------------------
	|                   |
	|         1         |
	|-------------------|
	|            |  /   |
	|     99     |_/____|
	|     99            |
	|                   |
	|      123          |
	--------------------
}



11-16
{
	Alright, some more things I'd like to add

	TODO(Eric): More statistics
		-

	TODO(Eric): Add Split functionality
		somwhere in the formatlabels, check if it's in a list of possible splits
		then display a version of the pin icon (8 with circle around it)

	TODO(Eric): Add a League Summary page
		have a League summary with all users a part of that league
		Also want to have a per-user league summary
			this could be in the user summary itself

	Future updates:
		Average Over Time
			Add a new table
				Date - Date of average
				Average
				UserID
			Have something execute to stamp another average there
				if (latest date > 1 week) stamp new average
				wouldn't need to be a new table actually
				just calculate average per the dates
		High Game Over Time
			Graph with highest game per week?
			I'd actually like to see games plotted out over time.
			This would actually tie in closely to Average Over Time

		Edit Multiple peoples Games at a time, in a series like format
			have an edit series page, but each game be game 1 of a different user
			would have to figure out permissions. who can do this?
				maybe time to actually set up Teams




	TODO(Eric): I'd like the previous button to skip the second throw when it's a strike
		if (all pins disabled) execute button again?
		DONE

		If I delete a game, and it's in a series, what happens?
			it actually just removes it from the series too
			the link is gone, so the series just has 1 less game

	TODO(Eric): Pagination for the Game page! getting a little long

	TODO(Eric): Redo deletion, so it doesn't actually delete anything
		Add a 'Previous UserID' to the game, set that, and set UserID to 0


}


12-19
{
	minified my files and reuploaded
		the previous arrow to skip strikes feature

}

12-20
{
    My Data access stuff is really messy; I need to go over it all
       I'm not mapping a lot of stuff in Game, Series tables etc..
       so when I query for them, I'm having to do a bunch of extra stuff
       to get the username, etc.
       I need to convert these to helper functions that gather all that data for me

}



1-2-2019
{
    Learning async/await a bit more
        await means that the function waits for that process to finish
            so my understanding is:
            you call a big query first
            then do a bunch of small work not depending on that query
            at the end, you do a 'await query.tolistasync'

    As for authorization
        the docs suggest that all controllers get an authorize policy by default
        then get a [AllowAnonymous] if I want them public
        supposedly that's more secure?
        
    The Logout button doesn't work
        it's something to do with the redirect afterwards?

    TODO: Combine the Stats page (MyStats) with the UserSummary page
          DONE pretty much - could use some additions

    Maybe add some extra info on the Game
        Bowled on date - so there's no amiguitiy on when it was actually bowled
            this will also provide a way to score previous games
            only trouble is them remembering all their frames
        Notes per game - ?
            ball change frame 3, moved left frame 7, etc. ?

        idk, i've been afraid to do database updates
        it will require a migration script to be generated and i don't want to corrupt anything

    actually, i think some of the bugs that i was seeing today were from running from the command line
    the global.json file was pointing to dotnet core 2.1, when i want 2.0 because of some bugs
    since it's not on azure, azure runs on 2.0, so it doesn't see those bugs
    and i'm pretty sure it's not run when i run it from visual studio
    
}

1-7
{
    Small bug fix - creating a series now shows the username on the edit page

    rethinking of adding Teams
               and this tab is way out here..

    ugh, so teams.. they kind of seem pointless in this app
    i don't want to have users keep track of all their teammates
    it would only be useful if i start tracking points and everything a league does

    damn i need a new project.
}


1-17
{
    I'm still craving a new project, but have none yet
    I started reading the Rust book some more, but don't know if I should continue at the moment
    CIS658 will have me learning Ruby and developing another full web app
    considering doing the bowling app again, just in Ruby for the class
        I'll have to ask the prof if I can do this, because I've done it before..

    Maybe I could start on my blog website / homepage and start blogging?
        Journey through learning Rust?
        Things I learn about D365?
        Things I learn about anything else?
        life blogs, such as bowling/mountain biking/gaming?

        you know, this file is supposed to be notes for just my bowling app
        the past few entries prove that I need another outlet to put my thoughts



}



1-21
{
    Alright, keeping with this project for now
    A few major things I want to do:

    Upgrade to latest dotnet core (2.2.1)
        Some Notes on this
             Look into redoing the Identity authentication stuff
             A lot of this is done for us in the newer versions?
             I need to see what I changed to it
                 Basically looks like the Email auth stuff
                 (In register and Login methods)
             Advantage of this is many files are removed

    Redo the front-end

    update all the javascript in the app to be typescript

    Do my database updates to add Teams



    end of day notes
        made some progress on updating to 2.2.1
        Identity isn't quite working (at least the old code for it is gone now)
        might want to update the login screen to use username again
              maybe not though?
        The login page is giving me a blank SignInResult
        

}

1-22
{
    Finish upgrading to dotnet core 2.2.1
        Login issue
              Might be from the input? username?




    Redo the front-end

    update all the javascript in the app to be typescript

    Do my database updates to add teams


    changelist so far
        updagraded to 2.2.1
        updated Identity to be the new 2.2 way
            still works with old logins, updated to include usernames
        fixing game duplication
            caused by a new game ID being set to 0, the tags aren't updated after it saves
            so then when it's saved again, the ID on the tag is still 0
            Now i'm saving the game on the Get request, which is a bad idea

            This should possibly be redone..
                For a series, SaveGameClick gets called 3 times, each time updating the series
                    Not terrible, but a little extra computation than needed
                All the checks for when a GameID is 0 seem silly
                    and are actually causing much more needing to be done
                

}

2-7
{
    something i thought of during class
    I might want to have a separate page between the list view and the other actions for it
        so for a game, i would just click on it to a landing page for that game
        then go into details, edit it, or delete it.
        maybe a modal popup of it? that would be nice..
    then I could make the tables a bit smaller, and the entire row clickable.

    experiment with position:fixed on the scoreboard?

    css rule to style by attribute:
        value = 10, text = 'X'
        
}

2-14
{
	Some more notes from during class

	I can probably reorganize my javascript to be much better
	event listener function being applied to all the frames
		when something changes, can try to rescore the game, etc.
		I know I'm practically doing this already, but..
		it could be cleaner
	all this comes along with, well it works currently, don't need to redo it all



}


2-19
{
	just some more random thoughts

	I should be creating my own homepage, with a blog

	add a progress bar for the series?
	with indicators for when the game/series is done?
	when they are done, semi lock them out?

	i think modals would work for what i said above (list pages, remove links, add modal)

	TODO: Combine the create pages of Game and Series
		checkbox: isSeries ?

		I could assume that if a league is selected, that it's a series with 3 games.
			BUT, what if they arrive late and only bowl 2 games?

		A lot of things here are possible, so..
		have league at the top, if they select one, default these fields:
			numofgames = 3
			isseries = yes
			

	TODO: Detect what day it is, and see if the user has a league on that day
			if so, have a quick create button for a series in that league

	

	NOTE: We updated the migrations locally, but not on Azure yet.
		Will need to research a bit how  Azure will try to handle it..
		

}


2-20
{
	NOTE: Azure Backups!
		everything costs money
		their way to do automatic backups is through a VM

		My Backups are located:
			Storage Account/Storage Container > Blob Containers > eric-container

		SQL Database Export puts the files there.
		Must be done manually

		I guess theres also a Restore option that goes back in time?
		
}

2-22
{
	TODO: If they leave the Game page, and current throw is 1, and score is 0
			then DELETE the game!

	I've been messing with modals, it's a little more complicated than i want it to be.

	Error page!
		I CAN actually pass the model to the view
		return View("Error", ErrorViewModel)
		!!


	sigh..long night

	So i spent hours messing with the IQueryable garbage, see comments
	it's still not getting leaguename
	

}

2-25
{
	fixed the league name issue in 5 minutes
		added a virtual League to the series and added a .include to the query
	
	So, in thinking of separating the series page and the game page..

		why don't i tie the series to the league itself?
		each league would have a series list
		the new button would be 'new league night'
			could then lock down what you can create for it
			only create a league's series if it's on the league night
			

	could have a landing page type thing for all of my games
		links to all series
		links to all games
		links to my leagues

	
	League message to display on the league when users view it

	User home page revamp
		upcoming leagues
		recent stuff

	If i were to re-add teams, I would almost like them to be optional

	TODO: Association table between Leagues and Users, also include TeamID

	When a User wants to join a league, they go to a 'current leagues' page..
		then they say they'd like to join one
			possibly have a user be a 'league owner/manager' to approve/deny these requests
		after they join one, populate the association table, and let them create series for it

		i really should lock down the amount of bowling they can do per league

		have it in the league setup how often they bowl, num of games, etc.
			then i can lock down the series and when they bowl one, default a lot of stuff in


	ok, so serius TODO list HERE:

	TODO: Team table and views going with that.
			possible other things to go with this
				Create a team upon creation of a league
			Teams are OPTIONAL for a league

	TODO: Association table between League, User, and Team

	TODO: Revamp User Home page
			Quick create a series for a league on the current day
			Recent activity
			my leagues
			links to 'all my series' and 'all my games'
				(I don't want the main 'all series/games' to be a place they always go)

	TODO: Lock down League with the amount of bowling users can do for them
			League new fields: 
				Occurance (every week, every other week)
				Number of games each week/per series
			Only allow users to bowl 1 series per occurance
				might have to tie this to series somehow? just do by bowl date?

	TODO: Add a Manager/Owner for each league
			Only this person can edit the league
			possible, in the future, have this person approve/deny users request to join
			also possibly have more than one person in this role
				(if the main owner is unresponsive or needs help)

	TODO: Add a League Message to display as a banner to all users for the league
			appear on the new User Home page per league
			explain what users can do with this league message

	TODO: Redo navigation and some page layouts:
		TODO: (Not priority) League summary page to be more of a landing page
			League night quick create button (if you are in it)
			

		TODO: admin(?) menu for edit pages
			League and location edit list, 

		TODO: (Not priority) redo league/location list to use list-group

		TODO: separate the series and games lists

		
	TODO: Style the list-group to be a different background color, and alternating rows
			maybe add a thick border too
			TODO: Actually, make this a tag helper/partial view
				to accept any kind of data
				pass in a special view model that is generic
				maybe the modal would be part of all this too
					would have to tell which type of record it's opening on the fly
			have a thick border with a nice title
			maybe generate my own theme with my own colors to user sitewide


	enough of a todo list for you?
	try to stick to it and make progress on them one at a time
			

}