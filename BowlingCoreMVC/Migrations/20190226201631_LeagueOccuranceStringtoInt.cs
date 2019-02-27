using Microsoft.EntityFrameworkCore.Migrations;

namespace BowlingCoreMVC.Migrations
{
    public partial class LeagueOccuranceStringtoInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Occurance",
                table: "Leagues",
                nullable: false,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Occurance",
                table: "Leagues",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
