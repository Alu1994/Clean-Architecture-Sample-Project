using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Migrations
{
    /// <inheritdoc />
    public partial class AddsCreationDateDefaultValueOnDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "UsersResources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 0, 44, 22, 958, DateTimeKind.Utc).AddTicks(8991),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 0, 44, 22, 958, DateTimeKind.Utc).AddTicks(5710),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Resources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 0, 44, 22, 958, DateTimeKind.Utc).AddTicks(7509),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "UsersResources",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 0, 44, 22, 958, DateTimeKind.Utc).AddTicks(8991));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 0, 44, 22, 958, DateTimeKind.Utc).AddTicks(5710));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Resources",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 0, 44, 22, 958, DateTimeKind.Utc).AddTicks(7509));
        }
    }
}
