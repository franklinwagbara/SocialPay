using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class createdaddtionalspectaloantable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddOrrInformationRequest",
                columns: table => new
                {
                    AddOrrInformationRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    maritalStatus = table.Column<int>(type: "int", nullable: false),
                    natureOfIncome = table.Column<int>(type: "int", nullable: false),
                    incomeSource = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    monthlyIncome = table.Column<int>(type: "int", nullable: false),
                    incomeSourceBusinessSegmentId = table.Column<int>(type: "int", nullable: false),
                    accommodationType = table.Column<int>(type: "int", nullable: false),
                    jobChanges = table.Column<int>(type: "int", nullable: false),
                    numberOfDependants = table.Column<int>(type: "int", nullable: false),
                    yearsInCurrentResidence = table.Column<int>(type: "int", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddOrrInformationRequest", x => x.AddOrrInformationRequestId);
                });

            migrationBuilder.CreateTable(
                name: "AddOrrInformationResponse",
                columns: table => new
                {
                    AddOrrInformationResponseId = table.Column<long>(type: "bigint", nullable: false)
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
                    table.PrimaryKey("PK_AddOrrInformationResponse", x => x.AddOrrInformationResponseId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddOrrInformationRequest");

            migrationBuilder.DropTable(
                name: "AddOrrInformationResponse");
        }
    }
}
