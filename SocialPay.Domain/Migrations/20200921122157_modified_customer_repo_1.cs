using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_customer_repo_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "CustomerTransaction");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ItemAcceptedOrRejected",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "OrderStatus",
                table: "CustomerTransaction",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "CustomerTransaction");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "ItemAcceptedOrRejected",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "CustomerTransaction",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
