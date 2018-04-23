ASP.NET Core MVC Bowling App
https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/?view=aspnetcore-2.0



Secrets
Storing secret values in the application the safe way:
	right-click project > Manage User Secrets
	This is our configuration info that won't get put into source control



Look into using a TagHelper for displaying frames
https://mva.microsoft.com/en-US/training-courses/aspnet-core-intermediate-18154?l=QiFcbpbeE_811787171



**In ASP CORE, use VIEW COMPONENTS instead of partial views**



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
