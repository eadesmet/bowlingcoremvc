using Microsoft.EntityFrameworkCore.Migrations;

namespace BowlingCoreMVC.Migrations
{
    public partial class idk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLeagueTeams_Teams_TeamID",
                table: "UserLeagueTeams");

            migrationBuilder.AlterColumn<int>(
                name: "TeamID",
                table: "UserLeagueTeams",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_UserLeagueTeams_Teams_TeamID",
                table: "UserLeagueTeams",
                column: "TeamID",
                principalTable: "Teams",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLeagueTeams_Teams_TeamID",
                table: "UserLeagueTeams");

            migrationBuilder.AlterColumn<int>(
                name: "TeamID",
                table: "UserLeagueTeams",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLeagueTeams_Teams_TeamID",
                table: "UserLeagueTeams",
                column: "TeamID",
                principalTable: "Teams",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
