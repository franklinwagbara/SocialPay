using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_dispute_table_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerTransactionReference",
                table: "DisputeRequestLog",
                newName: "ProcessedBy");

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "DisputeRequestLog",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "DisputeRequestLog");

            migrationBuilder.RenameColumn(
                name: "ProcessedBy",
                table: "DisputeRequestLog",
                newName: "CustomerTransactionReference");
        }
    }
}
