ASP.NET Core MVC Bowling App
https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/?view=aspnetcore-2.0



Secrets
Storing secret values in the application the safe way:
	right-click project > Manage User Secrets
	This is our configuration info that won't get put into source control



Look into using a TagHelper for displaying frames
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




So I don't like using the ViewModels everywhere
it seems like bad design that I have 3 different models for 1 db object

my idea now is to have EVERYTHING in my main model
would this have any issues?...


so it's one to many vs. many to one

a Game has a list of Frames, the frame has a GameID
	Game 1 --> * Frames

a League has a LocationID, locations have all possible locations
	League 1 <-- * Locations

