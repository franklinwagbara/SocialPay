using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class createdaddtionalspectaloantable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreateIndividualCurrentAccountRequest",
                columns: table => new
                {
                    CreateIndividualCurrentAccountRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherNationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdentityCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UtilityBill = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Passport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateIndividualCurrentAccountRequest", x => x.CreateIndividualCurrentAccountRequestId);
                });

            migrationBuilder.CreateTable(
                name: "CreateIndividualCurrentAccountResponse",
                columns: table => new
                {
                    CreateIndividualCurrentAccountResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    openedCurrentAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    success = table.Column<bool>(type: "bit", nullable: false),
                    unAuthorizedRequest = table.Column<bool>(type: "bit", nullable: false),
                    __abp = table.Column<bool>(type: "bit", nullable: false),
                    code = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    validationErrors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateIndividualCurrentAccountResponse", x => x.CreateIndividualCurrentAccountResponseId);
                });

            migrationBuilder.CreateTable(
                name: "SetDisbursementAccountRequest",
                columns: table => new
                {
                    SetDisbursementAccountRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    disbAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetDisbursementAccountRequest", x => x.SetDisbursementAccountRequestId);
                });

            migrationBuilder.CreateTable(
                name: "SetDisbursementAccountResponse",
                columns: table => new
                {
                    SetDisbursementAccountResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    success = table.Column<bool>(type: "bit", nullable: false),
                    unAuthorizedRequest = table.Column<bool>(type: "bit", nullable: false),
                    __abp = table.Column<bool>(type: "bit", nullable: false),
                    code = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    validationErrors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetDisbursementAccountResponse", x => x.SetDisbursementAccountResponseId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreateIndividualCurrentAccountRequest");

            migrationBuilder.DropTable(
                name: "CreateIndividualCurrentAccountResponse");

            migrationBuilder.DropTable(
                name: "SetDisbursementAccountRequest");

            migrationBuilder.DropTable(
                name: "SetDisbursementAccountResponse");
        }
    }
}
