using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeanBag.Migrations.TenantDb
{
    public partial class FinalMigrationTenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenantEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantTheme = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantSubscription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InviteCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.TenantId);
                });

            migrationBuilder.CreateTable(
                name: "TenantUser",
                columns: table => new
                {
                    UserObjectId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserTenantId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserRole = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUser", x => x.UserObjectId);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentRequestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropTable(
                name: "TenantUser");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
