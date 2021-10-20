using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class createdspectaregistrationtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpectaRegisterCustomerRequest",
                columns: table => new
                {
                    SpectaRegisterCustomerRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    surname = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    userName = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    emailAddress = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    password = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    dob = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    title = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    bvn = table.Column<string>(type: "NVARCHAR(11)", nullable: true),
                    phoneNumber = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    address = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    stateOfResidence = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    captchaResponse = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    RegistrationStatus = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpectaRegisterCustomerRequest", x => x.SpectaRegisterCustomerRequestId);
                    table.ForeignKey(
                        name: "FK_SpectaRegisterCustomerRequest_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpectaRegisterCustomerResponse",
                columns: table => new
                {
                    SpectaRegisterCustomerResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpectaRegisterCustomerRequestId = table.Column<long>(type: "bigint", nullable: false),
                    success = table.Column<bool>(type: "bit", nullable: false),
                    unAuthorizedRequest = table.Column<bool>(type: "bit", nullable: false),
                    __abp = table.Column<bool>(type: "bit", nullable: false),
                    code = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "NVARCHAR(290)", nullable: true),
                    details = table.Column<string>(type: "NVARCHAR(290)", nullable: true),
                    validationErrors = table.Column<string>(type: "NVARCHAR(290)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpectaRegisterCustomerResponse", x => x.SpectaRegisterCustomerResponseId);
                    table.ForeignKey(
                        name: "FK_SpectaRegisterCustomerResponse_SpectaRegisterCustomerRequest_SpectaRegisterCustomerRequestId",
                        column: x => x.SpectaRegisterCustomerRequestId,
                        principalTable: "SpectaRegisterCustomerRequest",
                        principalColumn: "SpectaRegisterCustomerRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpectaRegisterCustomerRequest_ClientAuthenticationId",
                table: "SpectaRegisterCustomerRequest",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_SpectaRegisterCustomerResponse_SpectaRegisterCustomerRequestId",
                table: "SpectaRegisterCustomerResponse",
                column: "SpectaRegisterCustomerRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpectaRegisterCustomerResponse");

            migrationBuilder.DropTable(
                name: "SpectaRegisterCustomerRequest");
        }
    }
}
