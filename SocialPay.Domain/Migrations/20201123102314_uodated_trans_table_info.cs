using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class uodated_trans_table_info : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AcitivityStatus",
                table: "TransactionLog",
                newName: "ActivityStatus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActivityStatus",
                table: "TransactionLog",
                newName: "AcitivityStatus");
        }
    }
}
