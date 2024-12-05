using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitectureSampleProject.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddsSellItemValueField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Sells",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 18, 8, 19, 526, DateTimeKind.Utc).AddTicks(2952),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(6573));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "SellItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 18, 8, 19, 526, DateTimeKind.Utc).AddTicks(4764),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(8308));

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "SellItems",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Products",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 18, 8, 19, 526, DateTimeKind.Utc).AddTicks(898),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(4670));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 18, 8, 19, 525, DateTimeKind.Utc).AddTicks(8967),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(2667));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "SellItems");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Sells",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(6573),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 18, 8, 19, 526, DateTimeKind.Utc).AddTicks(2952));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "SellItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(8308),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 18, 8, 19, 526, DateTimeKind.Utc).AddTicks(4764));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Products",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(4670),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 18, 8, 19, 526, DateTimeKind.Utc).AddTicks(898));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 1, 43, 1, 698, DateTimeKind.Utc).AddTicks(2667),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 18, 8, 19, 525, DateTimeKind.Utc).AddTicks(8967));
        }
    }
}
