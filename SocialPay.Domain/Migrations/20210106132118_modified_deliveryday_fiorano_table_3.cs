using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_deliveryday_fiorano_table_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrignatorName",
                table: "InterBankTransactionRequest",
                type: "NVARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(40)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NESessionID",
                table: "InterBankTransactionRequest",
                type: "NVARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(35)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "InterBankTransactionRequest",
                type: "NVARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(40)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountLockID",
                table: "InterBankTransactionRequest",
                type: "NVARCHAR(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(15)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrignatorName",
                table: "AcceptedEscrowInterBankTransactionRequest",
                type: "NVARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(40)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NESessionID",
                table: "AcceptedEscrowInterBankTransactionRequest",
                type: "NVARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(35)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "AcceptedEscrowInterBankTransactionRequest",
                type: "NVARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(40)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountLockID",
                table: "AcceptedEscrowInterBankTransactionRequest",
                type: "NVARCHAR(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(15)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrignatorName",
                table: "InterBankTransactionRequest",
                type: "NVARCHAR(40)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NESessionID",
                table: "InterBankTransactionRequest",
                type: "NVARCHAR(35)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "InterBankTransactionRequest",
                type: "NVARCHAR(40)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountLockID",
                table: "InterBankTransactionRequest",
                type: "NVARCHAR(15)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrignatorName",
                table: "AcceptedEscrowInterBankTransactionRequest",
                type: "NVARCHAR(40)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NESessionID",
                table: "AcceptedEscrowInterBankTransactionRequest",
                type: "NVARCHAR(35)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "AcceptedEscrowInterBankTransactionRequest",
                type: "NVARCHAR(40)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountLockID",
                table: "AcceptedEscrowInterBankTransactionRequest",
                type: "NVARCHAR(15)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(30)",
                oldNullable: true);
        }
    }
}
