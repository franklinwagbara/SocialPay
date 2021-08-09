using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class updatedairtimevendingtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "VendAirtimeRequestLog",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "VendAirtimeRequestLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Paymentcode",
                table: "VendAirtimeRequestLog",
                type: "NVARCHAR(45)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "VendAirtimeRequestLog");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "VendAirtimeRequestLog");

            migrationBuilder.DropColumn(
                name: "Paymentcode",
                table: "VendAirtimeRequestLog");
        }
    }
}
