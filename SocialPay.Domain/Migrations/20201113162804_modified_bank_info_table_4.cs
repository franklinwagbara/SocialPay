using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_bank_info_table_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CustomerId",
                table: "CustomerOtherPaymentsInfo",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "CustomerOtherPaymentsInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fullname",
                table: "CustomerOtherPaymentsInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "CustomerOtherPaymentsInfo",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "CustomerOtherPaymentsInfo");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "CustomerOtherPaymentsInfo");

            migrationBuilder.DropColumn(
                name: "Fullname",
                table: "CustomerOtherPaymentsInfo");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "CustomerOtherPaymentsInfo");
        }
    }
}
