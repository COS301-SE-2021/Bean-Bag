using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeanBag.Migrations
{
    public partial class updated_item_and_inventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "colour",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "condition",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "createdDate",
                table: "Inventories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "colour",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "condition",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "createdDate",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "description",
                table: "Inventories");
        }
    }
}
