using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class updatedaccountlookuptable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "merchantId",
                table: "DstvAccountLookupResponse");

            migrationBuilder.DropColumn(
                name: "merchantId",
                table: "DstvAccountLookup");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "merchantId",
                table: "DstvAccountLookupResponse",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "merchantId",
                table: "DstvAccountLookup",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
