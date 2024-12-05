using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitectureSampleProject.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddsSellEntitiesOnDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Products",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(4751),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(3071),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateTable(
                name: "Sells",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TotalValue = table.Column<decimal>(type: "numeric", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(6458))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sells", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SellItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SellId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(8006))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellItems_Sells_SellId",
                        column: x => x.SellId,
                        principalTable: "Sells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SellItems_ProductId",
                table: "SellItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SellItems_SellId",
                table: "SellItems",
                column: "SellId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SellItems");

            migrationBuilder.DropTable(
                name: "Sells");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Products",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(4751));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 12, 1, 0, 47, 46, 730, DateTimeKind.Utc).AddTicks(3071));
        }
    }
}
