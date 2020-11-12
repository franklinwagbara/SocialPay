using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_transaction_table_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MerchantClientInfo",
                table: "TransactionLog",
                newName: "CustomerInfo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerInfo",
                table: "TransactionLog",
                newName: "MerchantClientInfo");
        }
    }
}
