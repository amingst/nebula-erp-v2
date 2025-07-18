using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nebula.Services.Organizations.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_organizations_IsActive",
                table: "organizations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_organizations_OrganizationName",
                table: "organizations",
                column: "OrganizationName");

            migrationBuilder.CreateIndex(
                name: "IX_organizations_OwnerId",
                table: "organizations",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_organizations_OwnerId_IsActive",
                table: "organizations",
                columns: new[] { "OwnerId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_organizations_IsActive",
                table: "organizations");

            migrationBuilder.DropIndex(
                name: "IX_organizations_OrganizationName",
                table: "organizations");

            migrationBuilder.DropIndex(
                name: "IX_organizations_OwnerId",
                table: "organizations");

            migrationBuilder.DropIndex(
                name: "IX_organizations_OwnerId_IsActive",
                table: "organizations");
        }
    }
}
