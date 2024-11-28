using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Migrations
{
    /// <inheritdoc />
    public partial class ImplementsUsersResourcesNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanDelete",
                table: "UsersResources",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanDelete",
                table: "UsersResources");
        }
    }
}
