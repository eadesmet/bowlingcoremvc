using Microsoft.EntityFrameworkCore.Migrations;

namespace BowlingCoreMVC.Migrations
{
    public partial class TeamIDinSeries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserLeagueTeams_TeamID",
                table: "UserLeagueTeams",
                column: "TeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLeagueTeams_Teams_TeamID",
                table: "UserLeagueTeams",
                column: "TeamID",
                principalTable: "Teams",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLeagueTeams_Teams_TeamID",
                table: "UserLeagueTeams");

            migrationBuilder.DropIndex(
                name: "IX_UserLeagueTeams_TeamID",
                table: "UserLeagueTeams");
        }
    }
}
