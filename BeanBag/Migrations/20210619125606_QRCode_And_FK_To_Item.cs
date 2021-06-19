using Microsoft.EntityFrameworkCore.Migrations;

namespace BeanBag.Migrations
{
    public partial class QRCode_And_FK_To_Item : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "qrNumber",
                table: "Items",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "QRCodes",
                columns: table => new
                {
                    QrCodeNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QrContents = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QRCodes", x => x.QrCodeNumber);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_qrNumber",
                table: "Items",
                column: "qrNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_QRCodes_qrNumber",
                table: "Items",
                column: "qrNumber",
                principalTable: "QRCodes",
                principalColumn: "QrCodeNumber",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_QRCodes_qrNumber",
                table: "Items");

            migrationBuilder.DropTable(
                name: "QRCodes");

            migrationBuilder.DropIndex(
                name: "IX_Items_qrNumber",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "qrNumber",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
