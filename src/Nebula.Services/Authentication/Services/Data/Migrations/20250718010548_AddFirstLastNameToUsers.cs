using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nebula.Services.Authentication.Services.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFirstLastNameToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "auth",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "auth",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "auth",
                table: "users");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "auth",
                table: "users");
        }
    }
}
