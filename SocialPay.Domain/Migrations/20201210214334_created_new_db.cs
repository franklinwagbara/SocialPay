using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class created_new_db : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientAuthentication",
                columns: table => new
                {
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    UserName = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    FullName = table.Column<string>(type: "VARCHAR(45)", nullable: true),
                    ClientSecretHash = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ClientSecretSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    StatusCode = table.Column<string>(type: "VARCHAR(5)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    RoleName = table.Column<string>(type: "VARCHAR(25)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientAuthentication", x => x.ClientAuthenticationId);
                });

            migrationBuilder.CreateTable(
                name: "FailedTransactions",
                columns: table => new
                {
                    FailedTransactionsId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    Message = table.Column<string>(type: "VARCHAR(350)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FailedTransactions", x => x.FailedTransactionsId);
                });

            migrationBuilder.CreateTable(
                name: "FioranoT24CardCreditRequest",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: false),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    TransactionBranch = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    TransactionType = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    DebitAcctNo = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    DebitCurrency = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    CreditCurrency = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAccountNo = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    CommissionCode = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    VtellerAppID = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    narrations = table.Column<string>(type: "VARCHAR(130)", nullable: true),
                    SessionId = table.Column<string>(type: "VARCHAR(35)", nullable: true),
                    TrxnLocation = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    JsonRequest = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Channel = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    Message = table.Column<string>(type: "VARCHAR(230)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FioranoT24CardCreditRequest", x => x.PaymentReference);
                });

            migrationBuilder.CreateTable(
                name: "FioranoT24TransactionResponse",
                columns: table => new
                {
                    FioranoT24TransactionResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    ReferenceID = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    ResponseCode = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    ResponseText = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    Balance = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    COMMAMT = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    CHARGEAMT = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    FTID = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    JsonResponse = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FioranoT24TransactionResponse", x => x.FioranoT24TransactionResponseId);
                });

            migrationBuilder.CreateTable(
                name: "NonEscrowFioranoT24Request",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: false),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    TransactionBranch = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    TransactionType = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    DebitAcctNo = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    DebitCurrency = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    CreditCurrency = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAccountNo = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    CommissionCode = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    VtellerAppID = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    narrations = table.Column<string>(type: "VARCHAR(130)", nullable: true),
                    SessionId = table.Column<string>(type: "VARCHAR(35)", nullable: true),
                    TrxnLocation = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    JsonRequest = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Channel = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    Message = table.Column<string>(type: "VARCHAR(230)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonEscrowFioranoT24Request", x => x.PaymentReference);
                });

            migrationBuilder.CreateTable(
                name: "PaymentResponse",
                columns: table => new
                {
                    PaymentResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    Message = table.Column<string>(type: "VARCHAR(150)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentResponse", x => x.PaymentResponseId);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransferResponse",
                columns: table => new
                {
                    WalletTransferResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    message = table.Column<string>(type: "VARCHAR(120)", nullable: true),
                    response = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    responsedata = table.Column<string>(type: "VARCHAR(150)", nullable: true),
                    sent = table.Column<bool>(type: "bit", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransferResponse", x => x.WalletTransferResponseId);
                });

            migrationBuilder.CreateTable(
                name: "AcceptedEscrowFioranoT24Request",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: false),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    TransactionBranch = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    TransactionType = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    DebitAcctNo = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    DebitCurrency = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    CreditCurrency = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAccountNo = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    CommissionCode = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    VtellerAppID = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    narrations = table.Column<string>(type: "VARCHAR(110)", nullable: true),
                    SessionId = table.Column<string>(type: "VARCHAR(40)", nullable: true),
                    TrxnLocation = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    JsonRequest = table.Column<string>(type: "VARCHAR(150)", nullable: true),
                    Channel = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    Message = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcceptedEscrowFioranoT24Request", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_AcceptedEscrowFioranoT24Request_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AcceptedEscrowWalletTransferRequestLog",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: false),
                    RequestId = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    toacct = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    frmacct = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    remarks = table.Column<string>(type: "VARCHAR(130)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcceptedEscrowWalletTransferRequestLog", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_AcceptedEscrowWalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountResetRequest",
                columns: table => new
                {
                    AccountResetRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    Token = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountResetRequest", x => x.AccountResetRequestId);
                    table.ForeignKey(
                        name: "FK_AccountResetRequest_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientLoginStatus",
                columns: table => new
                {
                    ClientLoginStatusId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    LoginAttempt = table.Column<int>(type: "int", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientLoginStatus", x => x.ClientLoginStatusId);
                    table.ForeignKey(
                        name: "FK_ClientLoginStatus_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreateWalletResponse",
                columns: table => new
                {
                    CreateWalletResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    Message = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateWalletResponse", x => x.CreateWalletResponseId);
                    table.ForeignKey(
                        name: "FK_CreateWalletResponse_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditMerchantWalletTransferRequestLog",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: false),
                    RequestId = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    toacct = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    frmacct = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    remarks = table.Column<string>(type: "VARCHAR(130)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditMerchantWalletTransferRequestLog", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_CreditMerchantWalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DebitMerchantWalletTransferRequestLog",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: false),
                    RequestId = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    toacct = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    frmacct = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    remarks = table.Column<string>(type: "VARCHAR(130)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebitMerchantWalletTransferRequestLog", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_DebitMerchantWalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeclinedWalletTransferRequestLog",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: false),
                    RequestId = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    toacct = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    frmacct = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    remarks = table.Column<string>(type: "VARCHAR(130)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclinedWalletTransferRequestLog", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_DeclinedWalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefaultWalletTransferRequestLog",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: false),
                    RequestId = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    toacct = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    frmacct = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    remarks = table.Column<string>(type: "VARCHAR(130)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultWalletTransferRequestLog", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_DefaultWalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DisputeRequestLog",
                columns: table => new
                {
                    DisputeRequestLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    DisputeComment = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    DisputeFile = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    ProcessedBy = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    FileLocation = table.Column<string>(type: "VARCHAR(130)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisputeRequestLog", x => x.DisputeRequestLogId);
                    table.ForeignKey(
                        name: "FK_DisputeRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterBankTransactionRequest",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    OriginatorKYCLevel = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    BeneficiaryKYCLevel = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    BeneficiaryBankVerificationNumber = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    OriginatorBankVerificationNumber = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    AppID = table.Column<int>(type: "int", nullable: false),
                    AccountLockID = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    OriginatorAccountNumber = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    AccountNumber = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    AccountName = table.Column<string>(type: "VARCHAR(40)", nullable: true),
                    DestinationBankCode = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    OrignatorName = table.Column<string>(type: "VARCHAR(40)", nullable: true),
                    SubAcctVal = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    LedCodeVal = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    CurCodeVal = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    CusNumVal = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    BraCodeVal = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    Vat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentRef = table.Column<string>(type: "VARCHAR(120)", nullable: true),
                    NESessionID = table.Column<string>(type: "VARCHAR(35)", nullable: true),
                    ChannelCode = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterBankTransactionRequest", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_InterBankTransactionRequest_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoicePaymentLink",
                columns: table => new
                {
                    InvoicePaymentLinkId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    InvoiceName = table.Column<string>(type: "VARCHAR(40)", nullable: true),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    Qty = table.Column<long>(type: "bigint", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "VARCHAR(40)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TransactionStatus = table.Column<bool>(type: "bit", nullable: false),
                    ShippingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePaymentLink", x => x.InvoicePaymentLinkId);
                    table.ForeignKey(
                        name: "FK_InvoicePaymentLink_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemAcceptedOrRejected",
                columns: table => new
                {
                    ItemAcceptedOrRejectedId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerTransactionId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    Comment = table.Column<string>(type: "VARCHAR(180)", nullable: true),
                    Status = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    ProcessedBy = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    OrderStatus = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    IsReturned = table.Column<bool>(type: "bit", nullable: false),
                    ReturnedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAcceptedOrRejected", x => x.ItemAcceptedOrRejectedId);
                    table.ForeignKey(
                        name: "FK_ItemAcceptedOrRejected_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LinkCategory",
                columns: table => new
                {
                    LinkCategoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    Channel = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkCategory", x => x.LinkCategoryId);
                    table.ForeignKey(
                        name: "FK_LinkCategory_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoginAttemptHistory",
                columns: table => new
                {
                    LoginAttemptHistoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAttemptHistory", x => x.LoginAttemptHistoryId);
                    table.ForeignKey(
                        name: "FK_LoginAttemptHistory_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchantActivitySetup",
                columns: table => new
                {
                    MerchantActivitySetupId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    PayOrchargeMe = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    ReceiveEmail = table.Column<bool>(type: "bit", nullable: false),
                    WithinLagos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OutSideLagos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OutSideNigeria = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantActivitySetup", x => x.MerchantActivitySetupId);
                    table.ForeignKey(
                        name: "FK_MerchantActivitySetup_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchantBankInfo",
                columns: table => new
                {
                    MerchantBankInfoId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    BankName = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    BankCode = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    BranchCode = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    LedCode = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    Nuban = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    AccountName = table.Column<string>(type: "VARCHAR(35)", nullable: true),
                    Currency = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    BVN = table.Column<string>(type: "VARCHAR(12)", nullable: true),
                    Country = table.Column<string>(type: "VARCHAR(25)", nullable: true),
                    CusNum = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    KycLevel = table.Column<string>(type: "VARCHAR(5)", nullable: true),
                    DefaultAccount = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantBankInfo", x => x.MerchantBankInfoId);
                    table.ForeignKey(
                        name: "FK_MerchantBankInfo_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchantBusinessInfo",
                columns: table => new
                {
                    MerchantBusinessInfoId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    MerchantReferenceId = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    BusinessName = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    BusinessPhoneNumber = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    BusinessEmail = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    Country = table.Column<string>(type: "VARCHAR(25)", nullable: true),
                    Chargebackemail = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    Logo = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    FileLocation = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantBusinessInfo", x => x.MerchantBusinessInfoId);
                    table.ForeignKey(
                        name: "FK_MerchantBusinessInfo_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchantPaymentSetup",
                columns: table => new
                {
                    MerchantPaymentSetupId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentLinkName = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    MerchantDescription = table.Column<string>(type: "VARCHAR(120)", nullable: true),
                    CustomerDescription = table.Column<string>(type: "VARCHAR(120)", nullable: true),
                    MerchantAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HasAdditionalCharges = table.Column<bool>(type: "bit", nullable: false),
                    CustomUrl = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    DeliveryMethod = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    DeliveryTime = table.Column<long>(type: "bigint", nullable: false),
                    RedirectAfterPayment = table.Column<bool>(type: "bit", nullable: false),
                    AdditionalDetails = table.Column<string>(type: "VARCHAR(120)", nullable: true),
                    PaymentCategory = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    PaymentLinkUrl = table.Column<string>(type: "VARCHAR(150)", nullable: true),
                    Document = table.Column<string>(type: "VARCHAR(120)", nullable: true),
                    FileLocation = table.Column<string>(type: "VARCHAR(150)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantPaymentSetup", x => x.MerchantPaymentSetupId);
                    table.ForeignKey(
                        name: "FK_MerchantPaymentSetup_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchantWallet",
                columns: table => new
                {
                    MerchantWalletId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    Firstname = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    Lastname = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    Mobile = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    DoB = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    Gender = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    status = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantWallet", x => x.MerchantWalletId);
                    table.ForeignKey(
                        name: "FK_MerchantWallet_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PinRequest",
                columns: table => new
                {
                    PinRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    Pin = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    TokenSecret = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinRequest", x => x.PinRequestId);
                    table.ForeignKey(
                        name: "FK_PinRequest_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactionLog",
                columns: table => new
                {
                    TransactionLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerInfo = table.Column<long>(type: "bigint", nullable: false),
                    CustomerEmail = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    Category = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    PaymentChannel = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    OrderStatus = table.Column<string>(type: "VARCHAR(5)", nullable: true),
                    TransactionStatus = table.Column<string>(type: "VARCHAR(5)", nullable: true),
                    ActivityStatus = table.Column<string>(type: "VARCHAR(5)", nullable: true),
                    TransactionJourney = table.Column<string>(type: "VARCHAR(5)", nullable: true),
                    DeliveryDayTransferStatus = table.Column<string>(type: "VARCHAR(5)", nullable: true),
                    Message = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    LinkCategory = table.Column<string>(type: "VARCHAR(5)", nullable: true),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsQueuedPayWithCard = table.Column<bool>(type: "bit", nullable: false),
                    IsCompletedPayWithCard = table.Column<bool>(type: "bit", nullable: false),
                    IsWalletQueued = table.Column<bool>(type: "bit", nullable: false),
                    IsWalletCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    IsNotified = table.Column<bool>(type: "bit", nullable: false),
                    TransactionCompleted = table.Column<bool>(type: "bit", nullable: false),
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryFinalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateNotified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WalletFundDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcceptRejectLastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLog", x => x.TransactionLogId);
                    table.ForeignKey(
                        name: "FK_TransactionLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransferRequestLog",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: false),
                    RequestId = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    toacct = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    frmacct = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    remarks = table.Column<string>(type: "VARCHAR(130)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransferRequestLog", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_WalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoicePaymentInfo",
                columns: table => new
                {
                    InvoicePaymentInfoId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoicePaymentLinkId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    Email = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    Fullname = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    Channel = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    TransactionStatus = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    Message = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePaymentInfo", x => x.InvoicePaymentInfoId);
                    table.ForeignKey(
                        name: "FK_InvoicePaymentInfo_InvoicePaymentLink_InvoicePaymentLinkId",
                        column: x => x.InvoicePaymentLinkId,
                        principalTable: "InvoicePaymentLink",
                        principalColumn: "InvoicePaymentLinkId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerOtherPaymentsInfo",
                columns: table => new
                {
                    CustomerOtherPaymentsInfoId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    MerchantPaymentSetupId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerDescription = table.Column<string>(type: "VARCHAR(130)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Email = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    Fullname = table.Column<string>(type: "VARCHAR(60)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    Document = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    Channel = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    FileLocation = table.Column<string>(type: "VARCHAR(130)", nullable: true),
                    PaymentReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    PaymentStatus = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOtherPaymentsInfo", x => x.CustomerOtherPaymentsInfoId);
                    table.ForeignKey(
                        name: "FK_CustomerOtherPaymentsInfo_MerchantPaymentSetup_MerchantPaymentSetupId",
                        column: x => x.MerchantPaymentSetupId,
                        principalTable: "MerchantPaymentSetup",
                        principalColumn: "MerchantPaymentSetupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerTransaction",
                columns: table => new
                {
                    CustomerTransactionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantPaymentSetupId = table.Column<long>(type: "bigint", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerEmail = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    Channel = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    OrderStatus = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    Message = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "VARCHAR(90)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTransaction", x => x.CustomerTransactionId);
                    table.ForeignKey(
                        name: "FK_CustomerTransaction_MerchantPaymentSetup_MerchantPaymentSetupId",
                        column: x => x.MerchantPaymentSetupId,
                        principalTable: "MerchantPaymentSetup",
                        principalColumn: "MerchantPaymentSetupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedEscrowFioranoT24Request_ClientAuthenticationId",
                table: "AcceptedEscrowFioranoT24Request",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedEscrowWalletTransferRequestLog_ClientAuthenticationId",
                table: "AcceptedEscrowWalletTransferRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountResetRequest_ClientAuthenticationId",
                table: "AccountResetRequest",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientLoginStatus_ClientAuthenticationId",
                table: "ClientLoginStatus",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_CreateWalletResponse_ClientAuthenticationId",
                table: "CreateWalletResponse",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditMerchantWalletTransferRequestLog_ClientAuthenticationId",
                table: "CreditMerchantWalletTransferRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOtherPaymentsInfo_MerchantPaymentSetupId",
                table: "CustomerOtherPaymentsInfo",
                column: "MerchantPaymentSetupId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTransaction_MerchantPaymentSetupId",
                table: "CustomerTransaction",
                column: "MerchantPaymentSetupId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitMerchantWalletTransferRequestLog_ClientAuthenticationId",
                table: "DebitMerchantWalletTransferRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_DeclinedWalletTransferRequestLog_ClientAuthenticationId",
                table: "DeclinedWalletTransferRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultWalletTransferRequestLog_ClientAuthenticationId",
                table: "DefaultWalletTransferRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_DisputeRequestLog_ClientAuthenticationId",
                table: "DisputeRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_InterBankTransactionRequest_ClientAuthenticationId",
                table: "InterBankTransactionRequest",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePaymentInfo_InvoicePaymentLinkId",
                table: "InvoicePaymentInfo",
                column: "InvoicePaymentLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePaymentLink_ClientAuthenticationId",
                table: "InvoicePaymentLink",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAcceptedOrRejected_ClientAuthenticationId",
                table: "ItemAcceptedOrRejected",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkCategory_ClientAuthenticationId",
                table: "LinkCategory",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttemptHistory_ClientAuthenticationId",
                table: "LoginAttemptHistory",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantActivitySetup_ClientAuthenticationId",
                table: "MerchantActivitySetup",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantBankInfo_ClientAuthenticationId",
                table: "MerchantBankInfo",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantBusinessInfo_ClientAuthenticationId",
                table: "MerchantBusinessInfo",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantPaymentSetup_ClientAuthenticationId",
                table: "MerchantPaymentSetup",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantWallet_ClientAuthenticationId",
                table: "MerchantWallet",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_PinRequest_ClientAuthenticationId",
                table: "PinRequest",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLog_ClientAuthenticationId",
                table: "TransactionLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransferRequestLog_ClientAuthenticationId",
                table: "WalletTransferRequestLog",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcceptedEscrowFioranoT24Request");

            migrationBuilder.DropTable(
                name: "AcceptedEscrowWalletTransferRequestLog");

            migrationBuilder.DropTable(
                name: "AccountResetRequest");

            migrationBuilder.DropTable(
                name: "ClientLoginStatus");

            migrationBuilder.DropTable(
                name: "CreateWalletResponse");

            migrationBuilder.DropTable(
                name: "CreditMerchantWalletTransferRequestLog");

            migrationBuilder.DropTable(
                name: "CustomerOtherPaymentsInfo");

            migrationBuilder.DropTable(
                name: "CustomerTransaction");

            migrationBuilder.DropTable(
                name: "DebitMerchantWalletTransferRequestLog");

            migrationBuilder.DropTable(
                name: "DeclinedWalletTransferRequestLog");

            migrationBuilder.DropTable(
                name: "DefaultWalletTransferRequestLog");

            migrationBuilder.DropTable(
                name: "DisputeRequestLog");

            migrationBuilder.DropTable(
                name: "FailedTransactions");

            migrationBuilder.DropTable(
                name: "FioranoT24CardCreditRequest");

            migrationBuilder.DropTable(
                name: "FioranoT24TransactionResponse");

            migrationBuilder.DropTable(
                name: "InterBankTransactionRequest");

            migrationBuilder.DropTable(
                name: "InvoicePaymentInfo");

            migrationBuilder.DropTable(
                name: "ItemAcceptedOrRejected");

            migrationBuilder.DropTable(
                name: "LinkCategory");

            migrationBuilder.DropTable(
                name: "LoginAttemptHistory");

            migrationBuilder.DropTable(
                name: "MerchantActivitySetup");

            migrationBuilder.DropTable(
                name: "MerchantBankInfo");

            migrationBuilder.DropTable(
                name: "MerchantBusinessInfo");

            migrationBuilder.DropTable(
                name: "MerchantWallet");

            migrationBuilder.DropTable(
                name: "NonEscrowFioranoT24Request");

            migrationBuilder.DropTable(
                name: "PaymentResponse");

            migrationBuilder.DropTable(
                name: "PinRequest");

            migrationBuilder.DropTable(
                name: "TransactionLog");

            migrationBuilder.DropTable(
                name: "WalletTransferRequestLog");

            migrationBuilder.DropTable(
                name: "WalletTransferResponse");

            migrationBuilder.DropTable(
                name: "MerchantPaymentSetup");

            migrationBuilder.DropTable(
                name: "InvoicePaymentLink");

            migrationBuilder.DropTable(
                name: "ClientAuthentication");
        }
    }
}
