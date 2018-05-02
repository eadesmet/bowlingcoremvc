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
	set a flag and warn the user if they have made changes but haven't saved them before leaving the page
