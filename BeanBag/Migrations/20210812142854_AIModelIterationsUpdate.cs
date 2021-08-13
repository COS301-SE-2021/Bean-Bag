using Microsoft.EntityFrameworkCore.Migrations;

namespace BeanBag.Migrations
{
    public partial class AIModelIterationsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "predictionUrl",
                table: "AIModelIterations",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "predictionUrl",
                table: "AIModelIterations");
        }
    }
}
