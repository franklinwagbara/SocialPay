using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class createdloanentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanRepaymentPlan",
                columns: table => new
                {
                    LoanRepaymentPlanId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DailySalesPercentage = table.Column<double>(type: "float", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    PA = table.Column<double>(type: "float", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRepaymentPlan", x => x.LoanRepaymentPlanId);
                });

            migrationBuilder.CreateTable(
                name: "ApplyForLoan",
                columns: table => new
                {
                    ApplyForLoanId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    LoanRepaymentPlanId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsAttended = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsBadDebt = table.Column<bool>(type: "bit", nullable: false),
                    IsCardTokenized = table.Column<bool>(type: "bit", nullable: false),
                    TokenizationToken = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    TokenizationEmail = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    isCustomerClean = table.Column<bool>(type: "bit", nullable: false),
                    HaveSterlingBankAccount = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmTokenizationResponse = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    TokenizationReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    HaveSterlingBankBusinessAccount = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplyForLoan", x => x.ApplyForLoanId);
                    table.ForeignKey(
                        name: "FK_ApplyForLoan_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplyForLoan_LoanRepaymentPlan_LoanRepaymentPlanId",
                        column: x => x.LoanRepaymentPlanId,
                        principalTable: "LoanRepaymentPlan",
                        principalColumn: "LoanRepaymentPlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanDailyDeductionLog",
                columns: table => new
                {
                    LoanDailyDeductionLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoanDisbursementId = table.Column<long>(type: "bigint", nullable: false),
                    ApplyForLoanId = table.Column<long>(type: "bigint", nullable: false),
                    RepaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountDeducted = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OutstandingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DebittNuban = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: false),
                    TransactionBranch = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    TransactionType = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitAcctNo = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitCurrency = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    CreditCurrency = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitAmount = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    CreditAccountNo = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    CommissionCode = table.Column<string>(type: "NVARCHAR(80)", nullable: true),
                    VtellerAppID = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    narrations = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    SessionId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TrxnLocation = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    message = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanDailyDeductionLog", x => x.LoanDailyDeductionLogId);
                    table.ForeignKey(
                        name: "FK_LoanDailyDeductionLog_ApplyForLoan_ApplyForLoanId",
                        column: x => x.ApplyForLoanId,
                        principalTable: "ApplyForLoan",
                        principalColumn: "ApplyForLoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanDisbursement",
                columns: table => new
                {
                    LoanDisbursementId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplyForLoanId = table.Column<long>(type: "bigint", nullable: false),
                    disbusedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BankCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    nuban = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: false),
                    HaveStartedRepayment = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanDisbursement", x => x.LoanDisbursementId);
                    table.ForeignKey(
                        name: "FK_LoanDisbursement_ApplyForLoan_ApplyForLoanId",
                        column: x => x.ApplyForLoanId,
                        principalTable: "ApplyForLoan",
                        principalColumn: "ApplyForLoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "loanMonthlyDeductionLog",
                columns: table => new
                {
                    LoanMonthlyDeductionLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanDisbursementId = table.Column<long>(type: "bigint", nullable: false),
                    ApplyForLoanId = table.Column<long>(type: "bigint", nullable: false),
                    RepaymentAmount = table.Column<double>(type: "float", nullable: false),
                    AmountDeducted = table.Column<double>(type: "float", nullable: false),
                    DefaultBalance = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loanMonthlyDeductionLog", x => x.LoanMonthlyDeductionLogId);
                    table.ForeignKey(
                        name: "FK_loanMonthlyDeductionLog_ApplyForLoan_ApplyForLoanId",
                        column: x => x.ApplyForLoanId,
                        principalTable: "ApplyForLoan",
                        principalColumn: "ApplyForLoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplyForLoan_ClientAuthenticationId",
                table: "ApplyForLoan",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplyForLoan_LoanRepaymentPlanId",
                table: "ApplyForLoan",
                column: "LoanRepaymentPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDailyDeductionLog_ApplyForLoanId",
                table: "LoanDailyDeductionLog",
                column: "ApplyForLoanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDisbursement_ApplyForLoanId",
                table: "LoanDisbursement",
                column: "ApplyForLoanId");

            migrationBuilder.CreateIndex(
                name: "IX_loanMonthlyDeductionLog_ApplyForLoanId",
                table: "loanMonthlyDeductionLog",
                column: "ApplyForLoanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanDailyDeductionLog");

            migrationBuilder.DropTable(
                name: "LoanDisbursement");

            migrationBuilder.DropTable(
                name: "loanMonthlyDeductionLog");

            migrationBuilder.DropTable(
                name: "ApplyForLoan");

            migrationBuilder.DropTable(
                name: "LoanRepaymentPlan");
        }
    }
}
