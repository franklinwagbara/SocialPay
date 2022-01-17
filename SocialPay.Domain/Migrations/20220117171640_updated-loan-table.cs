using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class updatedloantable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanDailyDeductionLog_ApplyForLoan_ApplyForLoanId",
                table: "LoanDailyDeductionLog");

            migrationBuilder.DropForeignKey(
                name: "FK_loanMonthlyDeductionLog_ApplyForLoan_ApplyForLoanId",
                table: "loanMonthlyDeductionLog");

            migrationBuilder.DropIndex(
                name: "IX_loanMonthlyDeductionLog_ApplyForLoanId",
                table: "loanMonthlyDeductionLog");

            migrationBuilder.DropIndex(
                name: "IX_LoanDailyDeductionLog_ApplyForLoanId",
                table: "LoanDailyDeductionLog");

            migrationBuilder.DropColumn(
                name: "ApplyForLoanId",
                table: "loanMonthlyDeductionLog");

            migrationBuilder.DropColumn(
                name: "ClientAuthenticationId",
                table: "LoanDisbursement");

            migrationBuilder.DropColumn(
                name: "ApplyForLoanId",
                table: "LoanDailyDeductionLog");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ApplyForLoanId",
                table: "loanMonthlyDeductionLog",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ClientAuthenticationId",
                table: "LoanDisbursement",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ApplyForLoanId",
                table: "LoanDailyDeductionLog",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_loanMonthlyDeductionLog_ApplyForLoanId",
                table: "loanMonthlyDeductionLog",
                column: "ApplyForLoanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDailyDeductionLog_ApplyForLoanId",
                table: "LoanDailyDeductionLog",
                column: "ApplyForLoanId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanDailyDeductionLog_ApplyForLoan_ApplyForLoanId",
                table: "LoanDailyDeductionLog",
                column: "ApplyForLoanId",
                principalTable: "ApplyForLoan",
                principalColumn: "ApplyForLoanId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_loanMonthlyDeductionLog_ApplyForLoan_ApplyForLoanId",
                table: "loanMonthlyDeductionLog",
                column: "ApplyForLoanId",
                principalTable: "ApplyForLoan",
                principalColumn: "ApplyForLoanId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
