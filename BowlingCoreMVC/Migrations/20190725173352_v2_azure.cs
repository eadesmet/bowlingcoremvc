using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BowlingCoreMVC.Migrations
{
    public partial class v2_azure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Series",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamID",
                table: "Series",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByID",
                table: "Locations",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Leagues",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByID",
                table: "Leagues",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultNumOfGames",
                table: "Leagues",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Leagues",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Occurance",
                table: "Leagues",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Games",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LeagueID = table.Column<int>(nullable: false),
                    TeamName = table.Column<string>(nullable: true),
                    CreatedByID = table.Column<string>(maxLength: 450, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Teams_Leagues_LeagueID",
                        column: x => x.LeagueID,
                        principalTable: "Leagues",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLeagueTeams",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<string>(maxLength: 450, nullable: true),
                    LeagueID = table.Column<int>(nullable: false),
                    TeamID = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLeagueTeams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserLeagueTeams_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Series_LeagueID",
                table: "Series",
                column: "LeagueID");

            migrationBuilder.CreateIndex(
                name: "IX_Series_TeamID",
                table: "Series",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Series_UserID",
                table: "Series",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_UserID",
                table: "Games",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_LeagueID",
                table: "Teams",
                column: "LeagueID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLeagueTeams_TeamID",
                table: "UserLeagueTeams",
                column: "TeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_AspNetUsers_UserID",
                table: "Games",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Series_Leagues_LeagueID",
                table: "Series",
                column: "LeagueID",
                principalTable: "Leagues",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Series_Teams_TeamID",
                table: "Series",
                column: "TeamID",
                principalTable: "Teams",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Series_AspNetUsers_UserID",
                table: "Series",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_AspNetUsers_UserID",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Series_Leagues_LeagueID",
                table: "Series");

            migrationBuilder.DropForeignKey(
                name: "FK_Series_Teams_TeamID",
                table: "Series");

            migrationBuilder.DropForeignKey(
                name: "FK_Series_AspNetUsers_UserID",
                table: "Series");

            migrationBuilder.DropTable(
                name: "UserLeagueTeams");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Series_LeagueID",
                table: "Series");

            migrationBuilder.DropIndex(
                name: "IX_Series_TeamID",
                table: "Series");

            migrationBuilder.DropIndex(
                name: "IX_Series_UserID",
                table: "Series");

            migrationBuilder.DropIndex(
                name: "IX_Games_UserID",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "TeamID",
                table: "Series");

            migrationBuilder.DropColumn(
                name: "DefaultNumOfGames",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "Occurance",
                table: "Leagues");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Series",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByID",
                table: "Locations",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Leagues",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByID",
                table: "Leagues",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Games",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 450,
                oldNullable: true);
        }
    }
}
