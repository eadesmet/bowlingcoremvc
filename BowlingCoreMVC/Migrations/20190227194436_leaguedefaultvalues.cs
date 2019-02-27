using Microsoft.EntityFrameworkCore.Migrations;

namespace BowlingCoreMVC.Migrations
{
    public partial class leaguedefaultvalues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Occurance",
                table: "Leagues",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "DefaultNumOfGames",
                table: "Leagues",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Occurance",
                table: "Leagues",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "DefaultNumOfGames",
                table: "Leagues",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: 3);
        }
    }
}
