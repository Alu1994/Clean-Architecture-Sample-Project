using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Migrations
{
    /// <inheritdoc />
    public partial class AddsIndexesOnDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsersResources_UserId",
                table: "UsersResources");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "UsersResources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Resources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_UsersResources_UserId_ResourceId",
                table: "UsersResources",
                columns: new[] { "UserId", "ResourceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Name",
                table: "Resources",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsersResources_UserId_ResourceId",
                table: "UsersResources");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Name",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Resources_Name",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "UsersResources");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Resources");

            migrationBuilder.CreateIndex(
                name: "IX_UsersResources_UserId",
                table: "UsersResources",
                column: "UserId");
        }
    }
}
