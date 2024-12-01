using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitectureSampleProject.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesSellEntitiesOnDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalValue",
                table: "Sells",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Sells",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(6573),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(6458));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "SellItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(8308),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(8006));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Products",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(4670),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(4751));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(2667),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(3071));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalValue",
                table: "Sells",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Sells",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(6458),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(6573));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "SellItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(8006),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(8308));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Products",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(4751),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(4670));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(3071),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(2667));
        }
    }
}
