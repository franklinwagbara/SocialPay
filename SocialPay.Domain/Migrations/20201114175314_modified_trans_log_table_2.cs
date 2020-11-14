using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_trans_log_table_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "TransactionLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "CustomerOtherPaymentsInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PaymentStatus",
                table: "CustomerOtherPaymentsInfo",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "TransactionLog");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "CustomerOtherPaymentsInfo");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "CustomerOtherPaymentsInfo");
        }
    }
}
