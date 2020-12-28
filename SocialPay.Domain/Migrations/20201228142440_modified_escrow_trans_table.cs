using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_escrow_trans_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "narrations",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(530)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(110)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrxnLocation",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionType",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionBranch",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(40)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(230)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(250)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JsonRequest",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(950)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(150)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreditCurrency",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreditAccountNo",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(20)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "narrations",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(110)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(530)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrxnLocation",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionType",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionBranch",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(40)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(250)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(230)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JsonRequest",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(150)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(950)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreditCurrency",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreditAccountNo",
                table: "AcceptedEscrowFioranoT24Request",
                type: "NVARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(30)",
                oldNullable: true);
        }
    }
}
