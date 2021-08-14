using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeanBag.Migrations
{
    public partial class AIModelIterations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIModelIterations",
                columns: table => new
                {
                    iterationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    iterationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    availableToUser = table.Column<bool>(type: "bit", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    projectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIModelIterations", x => x.iterationId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIModelIterations");

            migrationBuilder.AlterColumn<string>(
                name: "projectId",
                table: "AIModels",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
