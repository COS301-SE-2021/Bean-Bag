using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeanBag.Migrations
{
    public partial class AIModel_TenantId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "tenantId",
                table: "AIModels",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tenantId",
                table: "AIModels");
        }
    }
}
