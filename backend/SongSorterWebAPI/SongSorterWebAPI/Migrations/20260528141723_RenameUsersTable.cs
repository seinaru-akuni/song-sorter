using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SongSorterWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "GoogleUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GoogleUsers",
                table: "GoogleUsers",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GoogleUsers",
                table: "GoogleUsers");

            migrationBuilder.RenameTable(
                name: "GoogleUsers",
                newName: "Users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");
        }
    }
}
