using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class UpdatedSpectaRegistrationtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpectaRegisterCustomerRequest_ClientAuthentication_ClientAuthenticationId",
                table: "SpectaRegisterCustomerRequest");

            migrationBuilder.DropIndex(
                name: "IX_SpectaRegisterCustomerRequest_ClientAuthenticationId",
                table: "SpectaRegisterCustomerRequest");

            migrationBuilder.DropColumn(
                name: "ClientAuthenticationId",
                table: "SpectaRegisterCustomerRequest");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClientAuthenticationId",
                table: "SpectaRegisterCustomerRequest",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_SpectaRegisterCustomerRequest_ClientAuthenticationId",
                table: "SpectaRegisterCustomerRequest",
                column: "ClientAuthenticationId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpectaRegisterCustomerRequest_ClientAuthentication_ClientAuthenticationId",
                table: "SpectaRegisterCustomerRequest",
                column: "ClientAuthenticationId",
                principalTable: "ClientAuthentication",
                principalColumn: "ClientAuthenticationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
