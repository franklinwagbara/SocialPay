using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_tran_table_logs_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChannelMode",
                table: "WalletTransferRequestLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WalletTransferResponse",
                columns: table => new
                {
                    WalletTransferResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WalletTransferRequestLogId = table.Column<long>(type: "bigint", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    responsedata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransferResponse", x => x.WalletTransferResponseId);
                    table.ForeignKey(
                        name: "FK_WalletTransferResponse_WalletTransferRequestLog_WalletTransferRequestLogId",
                        column: x => x.WalletTransferRequestLogId,
                        principalTable: "WalletTransferRequestLog",
                        principalColumn: "WalletTransferRequestLogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransferResponse_WalletTransferRequestLogId",
                table: "WalletTransferResponse",
                column: "WalletTransferRequestLogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletTransferResponse");

            migrationBuilder.DropColumn(
                name: "ChannelMode",
                table: "WalletTransferRequestLog");
        }
    }
}
