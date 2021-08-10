using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class updatedtenantprofiletable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "TenantProfile",
                type: "NVARCHAR(90)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                table: "TenantProfile",
                type: "NVARCHAR(90)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TenantProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "TenantProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "TenantProfile");

            migrationBuilder.DropColumn(
                name: "ClientSecret",
                table: "TenantProfile");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TenantProfile");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TenantProfile");
        }
    }
}
