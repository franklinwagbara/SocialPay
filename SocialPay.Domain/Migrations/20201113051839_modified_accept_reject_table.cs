using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_accept_reject_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                table: "ItemAcceptedOrRejected",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDateModified",
                table: "ItemAcceptedOrRejected",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OrderStatus",
                table: "ItemAcceptedOrRejected",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnedDate",
                table: "ItemAcceptedOrRejected",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReturned",
                table: "ItemAcceptedOrRejected");

            migrationBuilder.DropColumn(
                name: "LastDateModified",
                table: "ItemAcceptedOrRejected");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "ItemAcceptedOrRejected");

            migrationBuilder.DropColumn(
                name: "ReturnedDate",
                table: "ItemAcceptedOrRejected");
        }
    }
}
