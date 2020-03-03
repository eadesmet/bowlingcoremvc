using Microsoft.EntityFrameworkCore.Migrations;

namespace BowlingCoreMVC.Migrations
{
    public partial class additionalthrowinfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "BallID",
                table: "Frames",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "FeetPos",
                table: "Frames",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "MarkPos",
                table: "Frames",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BallID",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "FeetPos",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "MarkPos",
                table: "Frames");
        }
    }
}
