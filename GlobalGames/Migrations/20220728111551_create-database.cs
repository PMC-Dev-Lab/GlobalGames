using Microsoft.EntityFrameworkCore.Migrations;

namespace GlobalGames.Migrations
{
    public partial class createdatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Leads",
                newName: "Nome");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Users",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Leads",
                newName: "Name");
        }
    }
}
