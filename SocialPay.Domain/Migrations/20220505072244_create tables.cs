using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class createtables : Migration
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

            migrationBuilder.CreateTable(
                name: "ChargeCardRequest",
                columns: table => new
                {
                    ChargeCardRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cvv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    expiryMonth = table.Column<int>(type: "int", nullable: false),
                    expiryYear = table.Column<int>(type: "int", nullable: false),
                    currency = table.Column<int>(type: "int", nullable: false),
                    pin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeCardRequest", x => x.ChargeCardRequestId);
                });

            migrationBuilder.CreateTable(
                name: "ChargeCardResponse",
                columns: table => new
                {
                    ChargeCardResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    displayText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    authUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cardStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cardId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isValidCardForStatement = table.Column<bool>(type: "bit", nullable: false),
                    isSterling = table.Column<bool>(type: "bit", nullable: false),
                    success = table.Column<bool>(type: "bit", nullable: false),
                    unAuthorizedRequest = table.Column<bool>(type: "bit", nullable: false),
                    __abp = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeCardResponse", x => x.ChargeCardResponseId);
                });

            migrationBuilder.CreateTable(
                name: "ClientAuthentication",
                columns: table => new
                {
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bvn = table.Column<string>(type: "NVARCHAR(11)", nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    UserName = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    FullName = table.Column<string>(type: "NVARCHAR(55)", nullable: true),
                    ClientSecretHash = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ClientSecretSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    StatusCode = table.Column<string>(type: "NVARCHAR(5)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    RoleName = table.Column<string>(type: "NVARCHAR(25)", nullable: true),
                    ReferralCode = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    ReferCode = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    QrCodeStatus = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    HasRegisteredCompany = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientAuthentication", x => x.ClientAuthenticationId);
                });

            migrationBuilder.CreateTable(
                name: "ConfirmTicketRequest",
                columns: table => new
                {
                    ConfirmTicketRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ticketNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ticketPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customerId = table.Column<int>(type: "int", nullable: false),
                    bankId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    accountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmTicketRequest", x => x.ConfirmTicketRequestId);
                });

            migrationBuilder.CreateTable(
                name: "ConfirmTicketResponse",
                columns: table => new
                {
                    ConfirmTicketResponseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    shortDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    loanLimit = table.Column<double>(type: "float", nullable: false),
                    limitExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    success = table.Column<bool>(type: "bit", nullable: false),
                    unAuthorizedRequest = table.Column<bool>(type: "bit", nullable: false),
                    __abp = table.Column<bool>(type: "bit", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    validationErrors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmTicketResponse", x => x.ConfirmTicketResponseId);
                });

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
                name: "EventLog",
                columns: table => new
                {
                    EventLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    IpAddress = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    ModuleAccessed = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR(120)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLog", x => x.EventLogId);
                });

            migrationBuilder.CreateTable(
                name: "FailedTransactions",
                columns: table => new
                {
                    FailedTransactionsId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Message = table.Column<string>(type: "NVARCHAR(550)", nullable: true),
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
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TransactionBranch = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    TransactionType = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitAcctNo = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitCurrency = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CreditCurrency = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAccountNo = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    CommissionCode = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    VtellerAppID = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    narrations = table.Column<string>(type: "NVARCHAR(530)", nullable: true),
                    SessionId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TrxnLocation = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    JsonRequest = table.Column<string>(type: "NVARCHAR(950)", nullable: true),
                    Channel = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    Message = table.Column<string>(type: "NVARCHAR(230)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FioranoT24CardCreditRequest", x => x.PaymentReference);
                });

            migrationBuilder.CreateTable(
                name: "FioranoT24DeliveryDayRequest",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TransactionBranch = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    TransactionType = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitAcctNo = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitCurrency = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CreditCurrency = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAccountNo = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    CommissionCode = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    VtellerAppID = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    narrations = table.Column<string>(type: "NVARCHAR(530)", nullable: true),
                    SessionId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TrxnLocation = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    JsonRequest = table.Column<string>(type: "NVARCHAR(950)", nullable: true),
                    Channel = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    Message = table.Column<string>(type: "NVARCHAR(230)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FioranoT24DeliveryDayRequest", x => x.PaymentReference);
                });

            migrationBuilder.CreateTable(
                name: "FioranoT24TransactionResponse",
                columns: table => new
                {
                    FioranoT24TransactionResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ReferenceID = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ResponseCode = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    ResponseText = table.Column<string>(type: "NVARCHAR(280)", nullable: true),
                    Balance = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    COMMAMT = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CHARGEAMT = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    FTID = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    JsonResponse = table.Column<string>(type: "NVARCHAR(550)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FioranoT24TransactionResponse", x => x.FioranoT24TransactionResponseId);
                });

            migrationBuilder.CreateTable(
                name: "InventoryHistory",
                columns: table => new
                {
                    InventoryHistoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsAdded = table.Column<bool>(type: "bit", nullable: false),
                    IsUpdated = table.Column<bool>(type: "bit", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryHistory", x => x.InventoryHistoryId);
                });

            migrationBuilder.CreateTable(
                name: "LoanDailyDeductionLog",
                columns: table => new
                {
                    LoanDailyDeductionLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoanDisbursementId = table.Column<long>(type: "bigint", nullable: false),
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
                });

            migrationBuilder.CreateTable(
                name: "loanMonthlyDeductionLog",
                columns: table => new
                {
                    LoanMonthlyDeductionLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanDisbursementId = table.Column<long>(type: "bigint", nullable: false),
                    RepaymentAmount = table.Column<double>(type: "float", nullable: false),
                    AmountDeducted = table.Column<double>(type: "float", nullable: false),
                    DefaultBalance = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loanMonthlyDeductionLog", x => x.LoanMonthlyDeductionLogId);
                });

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
                name: "NonEscrowFioranoT24Request",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TransactionBranch = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    TransactionType = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitAcctNo = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitCurrency = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CreditCurrency = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAccountNo = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    CommissionCode = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    VtellerAppID = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    narrations = table.Column<string>(type: "NVARCHAR(530)", nullable: true),
                    SessionId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TrxnLocation = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    JsonRequest = table.Column<string>(type: "NVARCHAR(980)", nullable: true),
                    Channel = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    Message = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
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
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Message = table.Column<string>(type: "NVARCHAR(550)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentResponse", x => x.PaymentResponseId);
                });

            migrationBuilder.CreateTable(
                name: "ProductOption",
                columns: table => new
                {
                    OptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOption", x => x.OptionId);
                });

            migrationBuilder.CreateTable(
                name: "PurchasedProduct",
                columns: table => new
                {
                    PurchasedProductId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasedProduct", x => x.PurchasedProductId);
                });

            migrationBuilder.CreateTable(
                name: "RequestTicketRequest",
                columns: table => new
                {
                    RequestTicketRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    accountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bankId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customerId = table.Column<int>(type: "int", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTicketRequest", x => x.RequestTicketRequestId);
                });

            migrationBuilder.CreateTable(
                name: "RequestTicketResponse",
                columns: table => new
                {
                    RequestTicketResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    success = table.Column<bool>(type: "bit", nullable: false),
                    unAuthorizedRequest = table.Column<bool>(type: "bit", nullable: false),
                    __abp = table.Column<bool>(type: "bit", nullable: false),
                    code = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    validationErrors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    shortDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    requestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTicketResponse", x => x.RequestTicketResponseId);
                });

            migrationBuilder.CreateTable(
                name: "SendBvnPhoneVerificationCodeResponse",
                columns: table => new
                {
                    SendBvnPhoneVerificationCodeResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_SendBvnPhoneVerificationCodeResponse", x => x.SendBvnPhoneVerificationCodeResponseId);
                });

            migrationBuilder.CreateTable(
                name: "SendEmailVerificationCodeRequest",
                columns: table => new
                {
                    SendEmailVerificationCodeRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    clientBaseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    verificationCodeParameterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    emailParameterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendEmailVerificationCodeRequest", x => x.SendEmailVerificationCodeRequestId);
                });

            migrationBuilder.CreateTable(
                name: "SendEmailVerificationCodeResponse",
                columns: table => new
                {
                    SendEmailVerificationCodeResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_SendEmailVerificationCodeResponse", x => x.SendEmailVerificationCodeResponseId);
                });

            migrationBuilder.CreateTable(
                name: "SendOtpRequest",
                columns: table => new
                {
                    SendOtpRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    otp = table.Column<int>(type: "int", nullable: false),
                    cardId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendOtpRequest", x => x.SendOtpRequestId);
                });

            migrationBuilder.CreateTable(
                name: "SendOtpResponse",
                columns: table => new
                {
                    SendOtpResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    displayText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    authUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cardStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cardId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isValidCardForStatement = table.Column<bool>(type: "bit", nullable: false),
                    isSterling = table.Column<bool>(type: "bit", nullable: false),
                    success = table.Column<bool>(type: "bit", nullable: false),
                    unAuthorizedRequest = table.Column<bool>(type: "bit", nullable: false),
                    __abp = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendOtpResponse", x => x.SendOtpResponseId);
                });

            migrationBuilder.CreateTable(
                name: "SendPhoneRequest",
                columns: table => new
                {
                    SendPhoneRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    phoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cardId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendPhoneRequest", x => x.SendPhoneRequestId);
                });

            migrationBuilder.CreateTable(
                name: "SendPhoneResponse",
                columns: table => new
                {
                    SendPhoneResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    displayText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    authUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cardStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cardId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isValidCardForStatement = table.Column<bool>(type: "bit", nullable: false),
                    isSterling = table.Column<bool>(type: "bit", nullable: false),
                    success = table.Column<bool>(type: "bit", nullable: false),
                    unAuthorizedRequest = table.Column<bool>(type: "bit", nullable: false),
                    __abp = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendPhoneResponse", x => x.SendPhoneResponseId);
                });

            migrationBuilder.CreateTable(
                name: "SendPinRequest",
                columns: table => new
                {
                    SendPinRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cardId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendPinRequest", x => x.SendPinRequestId);
                });

            migrationBuilder.CreateTable(
                name: "SendPinResponse",
                columns: table => new
                {
                    SendPinResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    displayText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    authUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cardStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cardId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isValidCardForStatement = table.Column<bool>(type: "bit", nullable: false),
                    isSterling = table.Column<bool>(type: "bit", nullable: false),
                    success = table.Column<bool>(type: "bit", nullable: false),
                    unAuthorizedRequest = table.Column<bool>(type: "bit", nullable: false),
                    __abp = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendPinResponse", x => x.SendPinResponseId);
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

            migrationBuilder.CreateTable(
                name: "SpectaRegisterCustomerRequest",
                columns: table => new
                {
                    SpectaRegisterCustomerRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                });

            migrationBuilder.CreateTable(
                name: "SpectaStageVerificationPinRequest",
                columns: table => new
                {
                    SpectaStageVerificationPinRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    EnterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpectaStageVerificationPinRequest", x => x.SpectaStageVerificationPinRequestId);
                });

            migrationBuilder.CreateTable(
                name: "StoreCategory",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreCategory", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "ValidateChargeRequest",
                columns: table => new
                {
                    ValidateChargeRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cardId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidateChargeRequest", x => x.ValidateChargeRequestId);
                });

            migrationBuilder.CreateTable(
                name: "ValidateChargeResponse",
                columns: table => new
                {
                    ValidateChargeResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    displayText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    authUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cardStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cardId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isValidCardForStatement = table.Column<bool>(type: "bit", nullable: false),
                    isSterling = table.Column<bool>(type: "bit", nullable: false),
                    success = table.Column<bool>(type: "bit", nullable: false),
                    unAuthorizedRequest = table.Column<bool>(type: "bit", nullable: false),
                    __abp = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidateChargeResponse", x => x.ValidateChargeResponseId);
                });

            migrationBuilder.CreateTable(
                name: "VerifyBvnPhoneConfirmationCodeRequest",
                columns: table => new
                {
                    VerifyBvnPhoneConfirmationCodeRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerifyBvnPhoneConfirmationCodeRequest", x => x.VerifyBvnPhoneConfirmationCodeRequestId);
                });

            migrationBuilder.CreateTable(
                name: "VerifyBvnPhoneConfirmationCodeResponse",
                columns: table => new
                {
                    VerifyBvnPhoneConfirmationCodeResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_VerifyBvnPhoneConfirmationCodeResponse", x => x.VerifyBvnPhoneConfirmationCodeResponseId);
                });

            migrationBuilder.CreateTable(
                name: "VerifyEmailConfirmationCodeRequest",
                columns: table => new
                {
                    VerifyEmailConfirmationCodeRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerifyEmailConfirmationCodeRequest", x => x.VerifyEmailConfirmationCodeRequestId);
                });

            migrationBuilder.CreateTable(
                name: "VerifyEmailConfirmationCodeResponse",
                columns: table => new
                {
                    VerifyEmailConfirmationCodeResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_VerifyEmailConfirmationCodeResponse", x => x.VerifyEmailConfirmationCodeResponseId);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransferResponse",
                columns: table => new
                {
                    WalletTransferResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    message = table.Column<string>(type: "NVARCHAR(120)", nullable: true),
                    response = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    responsedata = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    sent = table.Column<bool>(type: "bit", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransferResponse", x => x.WalletTransferResponseId);
                });

            migrationBuilder.CreateTable(
                name: "WebHookTransactionRequestLog",
                columns: table => new
                {
                    WebHookTransactionRequestLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationType = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TimeStamp = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    MerchantName = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    MerchantNo = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    SubMerchantName = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    SubMerchantNo = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TransactionTime = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TransactionAmount = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    MerchantFee = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    ResidualAmount = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    TransactionType = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    OrderSn = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    OrderNo = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Sign = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebHookTransactionRequestLog", x => x.WebHookTransactionRequestLogId);
                });

            migrationBuilder.CreateTable(
                name: "AcceptedEscrowFioranoT24Request",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TransactionBranch = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    TransactionType = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitAcctNo = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitCurrency = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CreditCurrency = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAccountNo = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    CommissionCode = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    VtellerAppID = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    narrations = table.Column<string>(type: "NVARCHAR(530)", nullable: true),
                    SessionId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TrxnLocation = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    JsonRequest = table.Column<string>(type: "NVARCHAR(950)", nullable: true),
                    Channel = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    Message = table.Column<string>(type: "NVARCHAR(230)", nullable: true),
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
                name: "AcceptedEscrowInterBankTransactionRequest",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    OriginatorKYCLevel = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    BeneficiaryKYCLevel = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    BeneficiaryBankVerificationNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    OriginatorBankVerificationNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    AppID = table.Column<int>(type: "int", nullable: false),
                    AccountLockID = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    OriginatorAccountNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    AccountNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    AccountName = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    DestinationBankCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    OrignatorName = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    SubAcctVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    LedCodeVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CurCodeVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CusNumVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    BraCodeVal = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    Vat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentRef = table.Column<string>(type: "NVARCHAR(120)", nullable: true),
                    NESessionID = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    ChannelCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcceptedEscrowInterBankTransactionRequest", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_AcceptedEscrowInterBankTransactionRequest_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcceptedEscrowWalletTransferRequestLog",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    RequestId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    toacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    frmacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    remarks = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
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
                name: "AccountHistory",
                columns: table => new
                {
                    AccountHistoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientSecretHash = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ClientSecretSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountHistory", x => x.AccountHistoryId);
                    table.ForeignKey(
                        name: "FK_AccountHistory_ClientAuthentication_ClientAuthenticationId",
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
                    Token = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
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
                name: "CardTokenization",
                columns: table => new
                {
                    CardTokenizationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    fullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dob = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tokenType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    channel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cardMinExpiryInMonths = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    redirectUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bvn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    amount = table.Column<float>(type: "real", nullable: false),
                    reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    responseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTokenization", x => x.CardTokenizationId);
                    table.ForeignKey(
                        name: "FK_CardTokenization_ClientAuthentication_ClientAuthenticationId",
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
                    Message = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
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
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    RequestId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    toacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    frmacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    remarks = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
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
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    RequestId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    toacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    frmacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    remarks = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
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
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    RequestId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    toacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    frmacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    remarks = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
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
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    RequestId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    toacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    frmacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    remarks = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
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
                name: "DeliveryDayEscrowInterBankTransactionRequest",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    OriginatorKYCLevel = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    BeneficiaryKYCLevel = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    BeneficiaryBankVerificationNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    OriginatorBankVerificationNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    AppID = table.Column<int>(type: "int", nullable: false),
                    AccountLockID = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    OriginatorAccountNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    AccountNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    AccountName = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    DestinationBankCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    OrignatorName = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    SubAcctVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    LedCodeVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CurCodeVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CusNumVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    BraCodeVal = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    Vat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentRef = table.Column<string>(type: "NVARCHAR(120)", nullable: true),
                    NESessionID = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    ChannelCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryDayEscrowInterBankTransactionRequest", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_DeliveryDayEscrowInterBankTransactionRequest_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryDayWalletTransferRequestLog",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    RequestId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    toacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    frmacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    remarks = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryDayWalletTransferRequestLog", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_DeliveryDayWalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
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
                    DisputeComment = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    DisputeFile = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ProcessedBy = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    FileLocation = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
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
                name: "DstvAccountLookup",
                columns: table => new
                {
                    DstvAccountLookupId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    merchantReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    transactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vasId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    countryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DstvAccountLookup", x => x.DstvAccountLookupId);
                    table.ForeignKey(
                        name: "FK_DstvAccountLookup_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FioranoBillsRequest",
                columns: table => new
                {
                    FioranoBillsRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionBranch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillsType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitAcctNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditAccountNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommissionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VtellerAppID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    narrations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrxnLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FioranoBillsRequest", x => x.FioranoBillsRequestId);
                    table.ForeignKey(
                        name: "FK_FioranoBillsRequest_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuestAccountLog",
                columns: table => new
                {
                    GuestAccountLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestAccountLog", x => x.GuestAccountLogId);
                    table.ForeignKey(
                        name: "FK_GuestAccountLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterBankTransactionRequest",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    OriginatorKYCLevel = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    BeneficiaryKYCLevel = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    BeneficiaryBankVerificationNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    OriginatorBankVerificationNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    AppID = table.Column<int>(type: "int", nullable: false),
                    AccountLockID = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    OriginatorAccountNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    AccountNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    AccountName = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    DestinationBankCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    OrignatorName = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    SubAcctVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    LedCodeVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CurCodeVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CusNumVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    BraCodeVal = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    Vat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentRef = table.Column<string>(type: "NVARCHAR(120)", nullable: true),
                    NESessionID = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    ChannelCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
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
                    InvoiceName = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Qty = table.Column<long>(type: "bigint", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TransactionStatus = table.Column<bool>(type: "bit", nullable: false),
                    ShippingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VAT = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Comment = table.Column<string>(type: "NVARCHAR(180)", nullable: true),
                    Status = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    ProcessedBy = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    OrderStatus = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
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
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Channel = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
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
                    PayOrchargeMe = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
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
                    BankName = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    BankCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    BranchCode = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    LedCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    Nuban = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    AccountName = table.Column<string>(type: "NVARCHAR(65)", nullable: true),
                    Currency = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    BVN = table.Column<string>(type: "NVARCHAR(12)", nullable: true),
                    Country = table.Column<string>(type: "NVARCHAR(25)", nullable: true),
                    CusNum = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    KycLevel = table.Column<string>(type: "NVARCHAR(5)", nullable: true),
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
                name: "MerchantBureauSearch",
                columns: table => new
                {
                    MerchantBureauSearchId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    isCustomerClean = table.Column<bool>(type: "bit", nullable: false),
                    response = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantBureauSearch", x => x.MerchantBureauSearchId);
                    table.ForeignKey(
                        name: "FK_MerchantBureauSearch_ClientAuthentication_ClientAuthenticationId",
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
                    MerchantReferenceId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    SpectaMerchantID = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    SpectaMerchantKey = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    SpectaMerchantKeyValue = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    HasSpectaMerchantID = table.Column<bool>(type: "bit", nullable: false),
                    BusinessName = table.Column<string>(type: "NVARCHAR(65)", nullable: true),
                    BusinessPhoneNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    BusinessEmail = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    Country = table.Column<string>(type: "NVARCHAR(25)", nullable: true),
                    Tin = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    Chargebackemail = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    Logo = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    FileLocation = table.Column<string>(type: "NVARCHAR(190)", nullable: true),
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
                    MerchantStoreId = table.Column<long>(type: "bigint", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentLinkName = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    MerchantDescription = table.Column<string>(type: "NVARCHAR(120)", nullable: true),
                    CustomerDescription = table.Column<string>(type: "NVARCHAR(120)", nullable: true),
                    MerchantAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HasAdditionalCharges = table.Column<bool>(type: "bit", nullable: false),
                    CustomUrl = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    DeliveryMethod = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DeliveryTime = table.Column<long>(type: "bigint", nullable: false),
                    RedirectAfterPayment = table.Column<bool>(type: "bit", nullable: false),
                    AdditionalDetails = table.Column<string>(type: "NVARCHAR(120)", nullable: true),
                    PaymentCategory = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    PaymentLinkUrl = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    Document = table.Column<string>(type: "NVARCHAR(120)", nullable: true),
                    LinkCategory = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    FileLocation = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "MerchantQRCodeOnboarding",
                columns: table => new
                {
                    MerchantQRCodeOnboardingId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    Tin = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    Contact = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    Phone = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR(60)", nullable: true),
                    Address = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantQRCodeOnboarding", x => x.MerchantQRCodeOnboardingId);
                    table.ForeignKey(
                        name: "FK_MerchantQRCodeOnboarding_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchantStore",
                columns: table => new
                {
                    MerchantStoreId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    FileLocation = table.Column<string>(type: "NVARCHAR(190)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantStore", x => x.MerchantStoreId);
                    table.ForeignKey(
                        name: "FK_MerchantStore_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchantTransactionSetup",
                columns: table => new
                {
                    MerchantTransactionSetupId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    Pin = table.Column<string>(type: "NVARCHAR(355)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantTransactionSetup", x => x.MerchantTransactionSetupId);
                    table.ForeignKey(
                        name: "FK_MerchantTransactionSetup_ClientAuthentication_ClientAuthenticationId",
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
                    Firstname = table.Column<string>(type: "NVARCHAR(55)", nullable: true),
                    Lastname = table.Column<string>(type: "NVARCHAR(55)", nullable: true),
                    Mobile = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    DoB = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    Gender = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    status = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
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
                name: "OnboardingNotiification",
                columns: table => new
                {
                    OnboardingNotiificationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    notificationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnboardingNotiification", x => x.OnboardingNotiificationId);
                    table.ForeignKey(
                        name: "FK_OnboardingNotiification_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OtherMerchantBankInfo",
                columns: table => new
                {
                    MerchantOtherBankInfoId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    BankName = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    BankCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    BranchCode = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    LedCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    Nuban = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    AccountName = table.Column<string>(type: "NVARCHAR(65)", nullable: true),
                    Currency = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    BVN = table.Column<string>(type: "NVARCHAR(12)", nullable: true),
                    Country = table.Column<string>(type: "NVARCHAR(25)", nullable: true),
                    CusNum = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    KycLevel = table.Column<string>(type: "NVARCHAR(5)", nullable: true),
                    DefaultAccount = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherMerchantBankInfo", x => x.MerchantOtherBankInfoId);
                    table.ForeignKey(
                        name: "FK_OtherMerchantBankInfo_ClientAuthentication_ClientAuthenticationId",
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
                    Pin = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    TokenSecret = table.Column<string>(type: "NVARCHAR(350)", nullable: true),
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
                name: "ProductCategories",
                columns: table => new
                {
                    ProductCategoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.ProductCategoryId);
                    table.ForeignKey(
                        name: "FK_ProductCategories_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QrPaymentRequest",
                columns: table => new
                {
                    QrPaymentRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    OrderNo = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    OrderType = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    MchNo = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    SubMchNo = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    PaymentRequestReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrPaymentRequest", x => x.QrPaymentRequestId);
                    table.ForeignKey(
                        name: "FK_QrPaymentRequest_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SingleDstvPayment",
                columns: table => new
                {
                    SingleDstvPaymentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    AccountLookupReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    amountInCents = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    merchantId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    merchantReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    transactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vasId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    countryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SingleDstvPayment", x => x.SingleDstvPaymentId);
                    table.ForeignKey(
                        name: "FK_SingleDstvPayment_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreTransactionLog",
                columns: table => new
                {
                    StoreTransactionLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionStatus = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreTransactionLog", x => x.StoreTransactionLogId);
                    table.ForeignKey(
                        name: "FK_StoreTransactionLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantProfile",
                columns: table => new
                {
                    TenantProfileId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TenantName = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Address = table.Column<string>(type: "NVARCHAR(350)", nullable: true),
                    WebSiteUrl = table.Column<string>(type: "NVARCHAR(190)", nullable: true),
                    ClientId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ClientSecret = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    AuthKey = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantProfile", x => x.TenantProfileId);
                    table.ForeignKey(
                        name: "FK_TenantProfile_ClientAuthentication_ClientAuthenticationId",
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
                    CustomerEmail = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    Category = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    PaymentChannel = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    OrderStatus = table.Column<string>(type: "NVARCHAR(5)", nullable: true),
                    TransactionStatus = table.Column<string>(type: "NVARCHAR(5)", nullable: true),
                    ActivityStatus = table.Column<string>(type: "NVARCHAR(5)", nullable: true),
                    TransactionJourney = table.Column<string>(type: "NVARCHAR(5)", nullable: true),
                    DeliveryDayTransferStatus = table.Column<string>(type: "NVARCHAR(5)", nullable: true),
                    Message = table.Column<string>(type: "NVARCHAR(550)", nullable: true),
                    LinkCategory = table.Column<string>(type: "NVARCHAR(5)", nullable: true),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsQueuedPayWithCard = table.Column<bool>(type: "bit", nullable: false),
                    IsCompletedPayWithCard = table.Column<bool>(type: "bit", nullable: false),
                    IsWalletQueued = table.Column<bool>(type: "bit", nullable: false),
                    IsWalletCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    IsNotified = table.Column<bool>(type: "bit", nullable: false),
                    TransactionCompleted = table.Column<bool>(type: "bit", nullable: false),
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    OtherPaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TransactionType = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                name: "UssdServiceRequestLog",
                columns: table => new
                {
                    UssdServiceRequestLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    MerchantID = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    Channel = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    TransactionType = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    TransRef = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    MerchantName = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    TerminalId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    RetrievalReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    InstitutionCode = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ShortName = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Customer_mobile = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    SubMerchantName = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    UserID = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    ResponseCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    ResponseMessage = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    CallBackResponseCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CallBackResponseMessage = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    TransactionRef = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TransactionID = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TraceID = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UssdServiceRequestLog", x => x.UssdServiceRequestLogId);
                    table.ForeignKey(
                        name: "FK_UssdServiceRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendAirtimeRequestLog",
                columns: table => new
                {
                    VendAirtimeRequestLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    ReferenceId = table.Column<string>(type: "NVARCHAR(25)", nullable: true),
                    Translocation = table.Column<string>(type: "NVARCHAR(55)", nullable: true),
                    email = table.Column<string>(type: "NVARCHAR(55)", nullable: true),
                    SubscriberInfo1 = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    nuban = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    TransactionType = table.Column<string>(type: "NVARCHAR(45)", nullable: true),
                    AppId = table.Column<int>(type: "int", nullable: false),
                    RequestType = table.Column<int>(type: "int", nullable: false),
                    TerminalID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Paymentcode = table.Column<string>(type: "NVARCHAR(45)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendAirtimeRequestLog", x => x.VendAirtimeRequestLogId);
                    table.ForeignKey(
                        name: "FK_VendAirtimeRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransferRequestLog",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    RequestId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    toacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    frmacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    remarks = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
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
                name: "WebHookRequest",
                columns: table => new
                {
                    WebHookRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    description = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    webHookUri = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    filters = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    headers = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebHookRequest", x => x.WebHookRequestId);
                    table.ForeignKey(
                        name: "FK_WebHookRequest_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "MerchantStoreRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    OptionId = table.Column<int>(type: "int", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StoreCategoryCategoryId = table.Column<int>(type: "int", nullable: true),
                    ProductOptionOptionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantStoreRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MerchantStoreRequest_ProductOption_ProductOptionOptionId",
                        column: x => x.ProductOptionOptionId,
                        principalTable: "ProductOption",
                        principalColumn: "OptionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MerchantStoreRequest_StoreCategory_StoreCategoryCategoryId",
                        column: x => x.StoreCategoryCategoryId,
                        principalTable: "StoreCategory",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DstvAccountLookupResponse",
                columns: table => new
                {
                    DstvAccountLookupResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DstvAccountLookupId = table.Column<long>(type: "bigint", nullable: false),
                    merchantReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    payUVasReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    resultCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    resultMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pointOfFailure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DstvAccountLookupResponse", x => x.DstvAccountLookupResponseId);
                    table.ForeignKey(
                        name: "FK_DstvAccountLookupResponse_DstvAccountLookup_DstvAccountLookupId",
                        column: x => x.DstvAccountLookupId,
                        principalTable: "DstvAccountLookup",
                        principalColumn: "DstvAccountLookupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FioranoBillsPaymentResponse",
                columns: table => new
                {
                    FioranoBillsPaymentResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FioranoBillsRequestId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ReferenceID = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ResponseCode = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    ResponseText = table.Column<string>(type: "NVARCHAR(280)", nullable: true),
                    Balance = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    COMMAMT = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CHARGEAMT = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    FTID = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    JsonResponse = table.Column<string>(type: "NVARCHAR(550)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FioranoBillsPaymentResponse", x => x.FioranoBillsPaymentResponseId);
                    table.ForeignKey(
                        name: "FK_FioranoBillsPaymentResponse_FioranoBillsRequest_FioranoBillsRequestId",
                        column: x => x.FioranoBillsRequestId,
                        principalTable: "FioranoBillsRequest",
                        principalColumn: "FioranoBillsRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoicePaymentInfo",
                columns: table => new
                {
                    InvoicePaymentInfoId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoicePaymentLinkId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    Fullname = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    Channel = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    TransactionStatus = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    Message = table.Column<string>(type: "NVARCHAR(550)", nullable: true),
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
                name: "InvoicePaymentLinkToMulitpleEmails",
                columns: table => new
                {
                    InvoicePaymentLinkToMulitpleEmailsId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoicePaymentLinkId = table.Column<long>(type: "bigint", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePaymentLinkToMulitpleEmails", x => x.InvoicePaymentLinkToMulitpleEmailsId);
                    table.ForeignKey(
                        name: "FK_InvoicePaymentLinkToMulitpleEmails_InvoicePaymentLink_InvoicePaymentLinkId",
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
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    MerchantPaymentSetupId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerDescription = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    Fullname = table.Column<string>(type: "NVARCHAR(60)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    Document = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    Channel = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    FileLocation = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Category = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
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
                    CustomerEmail = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    Channel = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    OrderStatus = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    Message = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "BindMerchant",
                columns: table => new
                {
                    BindMerchantId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantQRCodeOnboardingId = table.Column<long>(type: "bigint", nullable: false),
                    MchNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BindMerchant", x => x.BindMerchantId);
                    table.ForeignKey(
                        name: "FK_BindMerchant_MerchantQRCodeOnboarding_MerchantQRCodeOnboardingId",
                        column: x => x.MerchantQRCodeOnboardingId,
                        principalTable: "MerchantQRCodeOnboarding",
                        principalColumn: "MerchantQRCodeOnboardingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchantQRCodeOnboardingResponse",
                columns: table => new
                {
                    MerchantQRCodeOnboardingResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantQRCodeOnboardingId = table.Column<long>(type: "bigint", nullable: false),
                    ReturnCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    ReturnMsg = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    MchNo = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    MerchantName = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    MerchantTIN = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    MerchantAddress = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    MerchantContactName = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    MerchantPhoneNumber = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    MerchantEmail = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    JsonResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantQRCodeOnboardingResponse", x => x.MerchantQRCodeOnboardingResponseId);
                    table.ForeignKey(
                        name: "FK_MerchantQRCodeOnboardingResponse_MerchantQRCodeOnboarding_MerchantQRCodeOnboardingId",
                        column: x => x.MerchantQRCodeOnboardingId,
                        principalTable: "MerchantQRCodeOnboarding",
                        principalColumn: "MerchantQRCodeOnboardingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubMerchantQRCodeOnboarding",
                columns: table => new
                {
                    SubMerchantQRCodeOnboardingId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantQRCodeOnboardingId = table.Column<long>(type: "bigint", nullable: false),
                    MchNo = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    MerchantName = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    MerchantEmail = table.Column<string>(type: "NVARCHAR(80)", nullable: true),
                    MerchantPhoneNumber = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    SubFixed = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    SubAmount = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubMerchantQRCodeOnboarding", x => x.SubMerchantQRCodeOnboardingId);
                    table.ForeignKey(
                        name: "FK_SubMerchantQRCodeOnboarding_MerchantQRCodeOnboarding_MerchantQRCodeOnboardingId",
                        column: x => x.MerchantQRCodeOnboardingId,
                        principalTable: "MerchantQRCodeOnboarding",
                        principalColumn: "MerchantQRCodeOnboardingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    MerchantStoreId = table.Column<long>(type: "bigint", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Options = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: true),
                    InventoryHistoryId = table.Column<long>(type: "bigint", nullable: true),
                    PurchasedProductId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_InventoryHistory_InventoryHistoryId",
                        column: x => x.InventoryHistoryId,
                        principalTable: "InventoryHistory",
                        principalColumn: "InventoryHistoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "ProductCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_PurchasedProduct_PurchasedProductId",
                        column: x => x.PurchasedProductId,
                        principalTable: "PurchasedProduct",
                        principalColumn: "PurchasedProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QrPaymentResponse",
                columns: table => new
                {
                    QrPaymentResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QrPaymentRequestId = table.Column<long>(type: "bigint", nullable: false),
                    OrderSn = table.Column<string>(type: "NVARCHAR(190)", nullable: true),
                    CodeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnCode = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ReturnMsg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    SessionID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrPaymentResponse", x => x.QrPaymentResponseId);
                    table.ForeignKey(
                        name: "FK_QrPaymentResponse_QrPaymentRequest_QrPaymentRequestId",
                        column: x => x.QrPaymentRequestId,
                        principalTable: "QrPaymentRequest",
                        principalColumn: "QrPaymentRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SingleDstvPaymentResponse",
                columns: table => new
                {
                    SingleDstvPaymentResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SingleDstvPaymentId = table.Column<long>(type: "bigint", nullable: false),
                    merchantReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    payUVasReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    resultCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    resultMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pointOfFailure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    merchantId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SingleDstvPaymentResponse", x => x.SingleDstvPaymentResponseId);
                    table.ForeignKey(
                        name: "FK_SingleDstvPaymentResponse_SingleDstvPayment_SingleDstvPaymentId",
                        column: x => x.SingleDstvPaymentId,
                        principalTable: "SingleDstvPayment",
                        principalColumn: "SingleDstvPaymentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreTransactionLogDetails",
                columns: table => new
                {
                    StoreTransactionLogDetailsId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreTransactionLogId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    Size = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionStatus = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreTransactionLogDetails", x => x.StoreTransactionLogDetailsId);
                    table.ForeignKey(
                        name: "FK_StoreTransactionLogDetails_StoreTransactionLog_StoreTransactionLogId",
                        column: x => x.StoreTransactionLogId,
                        principalTable: "StoreTransactionLog",
                        principalColumn: "StoreTransactionLogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanDisbursement",
                columns: table => new
                {
                    LoanDisbursementId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                name: "BindMerchantResponse",
                columns: table => new
                {
                    BindMerchantResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BindMerchantId = table.Column<long>(type: "bigint", nullable: false),
                    ReturnCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mch_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JsonResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BindMerchantResponse", x => x.BindMerchantResponseId);
                    table.ForeignKey(
                        name: "FK_BindMerchantResponse_BindMerchant_BindMerchantId",
                        column: x => x.BindMerchantId,
                        principalTable: "BindMerchant",
                        principalColumn: "BindMerchantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubMerchantQRCodeOnboardingResponse",
                columns: table => new
                {
                    SubMerchantQRCodeOnboardingResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    SubMerchantQRCodeOnboardingId = table.Column<long>(type: "bigint", nullable: false),
                    ReturnCode = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    ReturnMsg = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    MchNo = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    MerchantName = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    SubMchNo = table.Column<string>(type: "NVARCHAR(60)", nullable: true),
                    QrCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JsonResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubMerchantQRCodeOnboardingResponse", x => x.SubMerchantQRCodeOnboardingResponseId);
                    table.ForeignKey(
                        name: "FK_SubMerchantQRCodeOnboardingResponse_SubMerchantQRCodeOnboarding_SubMerchantQRCodeOnboardingId",
                        column: x => x.SubMerchantQRCodeOnboardingId,
                        principalTable: "SubMerchantQRCodeOnboarding",
                        principalColumn: "SubMerchantQRCodeOnboardingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductInventory",
                columns: table => new
                {
                    ProductInventoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInventory", x => x.ProductInventoryId);
                    table.ForeignKey(
                        name: "FK_ProductInventory_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductItems",
                columns: table => new
                {
                    ProductItemsId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    FileLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductItems", x => x.ProductItemsId);
                    table.ForeignKey(
                        name: "FK_ProductItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "productInventoryHistories",
                columns: table => new
                {
                    ProductInventoryHistoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdId = table.Column<long>(type: "bigint", nullable: false),
                    ProductInventoryId = table.Column<long>(type: "bigint", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsAdded = table.Column<bool>(type: "bit", nullable: false),
                    IsUpdated = table.Column<bool>(type: "bit", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productInventoryHistories", x => x.ProductInventoryHistoryId);
                    table.ForeignKey(
                        name: "FK_productInventoryHistories_ProductInventory_ProductInventoryId",
                        column: x => x.ProductInventoryId,
                        principalTable: "ProductInventory",
                        principalColumn: "ProductInventoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedEscrowFioranoT24Request_ClientAuthenticationId",
                table: "AcceptedEscrowFioranoT24Request",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedEscrowInterBankTransactionRequest_ClientAuthenticationId",
                table: "AcceptedEscrowInterBankTransactionRequest",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedEscrowWalletTransferRequestLog_ClientAuthenticationId",
                table: "AcceptedEscrowWalletTransferRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistory_ClientAuthenticationId",
                table: "AccountHistory",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountResetRequest_ClientAuthenticationId",
                table: "AccountResetRequest",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplyForLoan_ClientAuthenticationId",
                table: "ApplyForLoan",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplyForLoan_LoanRepaymentPlanId",
                table: "ApplyForLoan",
                column: "LoanRepaymentPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_BindMerchant_MerchantQRCodeOnboardingId",
                table: "BindMerchant",
                column: "MerchantQRCodeOnboardingId");

            migrationBuilder.CreateIndex(
                name: "IX_BindMerchantResponse_BindMerchantId",
                table: "BindMerchantResponse",
                column: "BindMerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_CardTokenization_ClientAuthenticationId",
                table: "CardTokenization",
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
                name: "IX_DeliveryDayEscrowInterBankTransactionRequest_ClientAuthenticationId",
                table: "DeliveryDayEscrowInterBankTransactionRequest",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDayWalletTransferRequestLog_ClientAuthenticationId",
                table: "DeliveryDayWalletTransferRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_DisputeRequestLog_ClientAuthenticationId",
                table: "DisputeRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_DstvAccountLookup_ClientAuthenticationId",
                table: "DstvAccountLookup",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_DstvAccountLookupResponse_DstvAccountLookupId",
                table: "DstvAccountLookupResponse",
                column: "DstvAccountLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_FioranoBillsPaymentResponse_FioranoBillsRequestId",
                table: "FioranoBillsPaymentResponse",
                column: "FioranoBillsRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_FioranoBillsRequest_ClientAuthenticationId",
                table: "FioranoBillsRequest",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_GuestAccountLog_ClientAuthenticationId",
                table: "GuestAccountLog",
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
                name: "IX_InvoicePaymentLinkToMulitpleEmails_InvoicePaymentLinkId",
                table: "InvoicePaymentLinkToMulitpleEmails",
                column: "InvoicePaymentLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAcceptedOrRejected_ClientAuthenticationId",
                table: "ItemAcceptedOrRejected",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkCategory_ClientAuthenticationId",
                table: "LinkCategory",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDisbursement_ApplyForLoanId",
                table: "LoanDisbursement",
                column: "ApplyForLoanId");

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
                name: "IX_MerchantBureauSearch_ClientAuthenticationId",
                table: "MerchantBureauSearch",
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
                name: "IX_MerchantQRCodeOnboarding_ClientAuthenticationId",
                table: "MerchantQRCodeOnboarding",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantQRCodeOnboardingResponse_MerchantQRCodeOnboardingId",
                table: "MerchantQRCodeOnboardingResponse",
                column: "MerchantQRCodeOnboardingId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantStore_ClientAuthenticationId",
                table: "MerchantStore",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantStoreRequest_ProductOptionOptionId",
                table: "MerchantStoreRequest",
                column: "ProductOptionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantStoreRequest_StoreCategoryCategoryId",
                table: "MerchantStoreRequest",
                column: "StoreCategoryCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantTransactionSetup_ClientAuthenticationId",
                table: "MerchantTransactionSetup",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantWallet_ClientAuthenticationId",
                table: "MerchantWallet",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingNotiification_ClientAuthenticationId",
                table: "OnboardingNotiification",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherMerchantBankInfo_ClientAuthenticationId",
                table: "OtherMerchantBankInfo",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_PinRequest_ClientAuthenticationId",
                table: "PinRequest",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ClientAuthenticationId",
                table: "ProductCategories",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductInventory_ProductId",
                table: "ProductInventory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_productInventoryHistories_ProductInventoryId",
                table: "productInventoryHistories",
                column: "ProductInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductItems_ProductId",
                table: "ProductItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ClientAuthenticationId",
                table: "Products",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_InventoryHistoryId",
                table: "Products",
                column: "InventoryHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCategoryId",
                table: "Products",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_PurchasedProductId",
                table: "Products",
                column: "PurchasedProductId");

            migrationBuilder.CreateIndex(
                name: "IX_QrPaymentRequest_ClientAuthenticationId",
                table: "QrPaymentRequest",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_QrPaymentResponse_QrPaymentRequestId",
                table: "QrPaymentResponse",
                column: "QrPaymentRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_SingleDstvPayment_ClientAuthenticationId",
                table: "SingleDstvPayment",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_SingleDstvPaymentResponse_SingleDstvPaymentId",
                table: "SingleDstvPaymentResponse",
                column: "SingleDstvPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_SpectaRegisterCustomerResponse_SpectaRegisterCustomerRequestId",
                table: "SpectaRegisterCustomerResponse",
                column: "SpectaRegisterCustomerRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransactionLog_ClientAuthenticationId",
                table: "StoreTransactionLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransactionLogDetails_StoreTransactionLogId",
                table: "StoreTransactionLogDetails",
                column: "StoreTransactionLogId");

            migrationBuilder.CreateIndex(
                name: "IX_SubMerchantQRCodeOnboarding_MerchantQRCodeOnboardingId",
                table: "SubMerchantQRCodeOnboarding",
                column: "MerchantQRCodeOnboardingId");

            migrationBuilder.CreateIndex(
                name: "IX_SubMerchantQRCodeOnboardingResponse_SubMerchantQRCodeOnboardingId",
                table: "SubMerchantQRCodeOnboardingResponse",
                column: "SubMerchantQRCodeOnboardingId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantProfile_ClientAuthenticationId",
                table: "TenantProfile",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLog_ClientAuthenticationId",
                table: "TransactionLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_UssdServiceRequestLog_ClientAuthenticationId",
                table: "UssdServiceRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_VendAirtimeRequestLog_ClientAuthenticationId",
                table: "VendAirtimeRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransferRequestLog_ClientAuthenticationId",
                table: "WalletTransferRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_WebHookRequest_ClientAuthenticationId",
                table: "WebHookRequest",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcceptedEscrowFioranoT24Request");

            migrationBuilder.DropTable(
                name: "AcceptedEscrowInterBankTransactionRequest");

            migrationBuilder.DropTable(
                name: "AcceptedEscrowWalletTransferRequestLog");

            migrationBuilder.DropTable(
                name: "AccountHistory");

            migrationBuilder.DropTable(
                name: "AccountResetRequest");

            migrationBuilder.DropTable(
                name: "AddOrrInformationRequest");

            migrationBuilder.DropTable(
                name: "AddOrrInformationResponse");

            migrationBuilder.DropTable(
                name: "BindMerchantResponse");

            migrationBuilder.DropTable(
                name: "CardTokenization");

            migrationBuilder.DropTable(
                name: "ChargeCardRequest");

            migrationBuilder.DropTable(
                name: "ChargeCardResponse");

            migrationBuilder.DropTable(
                name: "ClientLoginStatus");

            migrationBuilder.DropTable(
                name: "ConfirmTicketRequest");

            migrationBuilder.DropTable(
                name: "ConfirmTicketResponse");

            migrationBuilder.DropTable(
                name: "CreateIndividualCurrentAccountRequest");

            migrationBuilder.DropTable(
                name: "CreateIndividualCurrentAccountResponse");

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
                name: "DeliveryDayEscrowInterBankTransactionRequest");

            migrationBuilder.DropTable(
                name: "DeliveryDayWalletTransferRequestLog");

            migrationBuilder.DropTable(
                name: "DisputeRequestLog");

            migrationBuilder.DropTable(
                name: "DstvAccountLookupResponse");

            migrationBuilder.DropTable(
                name: "EventLog");

            migrationBuilder.DropTable(
                name: "FailedTransactions");

            migrationBuilder.DropTable(
                name: "FioranoBillsPaymentResponse");

            migrationBuilder.DropTable(
                name: "FioranoT24CardCreditRequest");

            migrationBuilder.DropTable(
                name: "FioranoT24DeliveryDayRequest");

            migrationBuilder.DropTable(
                name: "FioranoT24TransactionResponse");

            migrationBuilder.DropTable(
                name: "GuestAccountLog");

            migrationBuilder.DropTable(
                name: "InterBankTransactionRequest");

            migrationBuilder.DropTable(
                name: "InvoicePaymentInfo");

            migrationBuilder.DropTable(
                name: "InvoicePaymentLinkToMulitpleEmails");

            migrationBuilder.DropTable(
                name: "ItemAcceptedOrRejected");

            migrationBuilder.DropTable(
                name: "LinkCategory");

            migrationBuilder.DropTable(
                name: "LoanDailyDeductionLog");

            migrationBuilder.DropTable(
                name: "LoanDisbursement");

            migrationBuilder.DropTable(
                name: "loanMonthlyDeductionLog");

            migrationBuilder.DropTable(
                name: "LoginAttemptHistory");

            migrationBuilder.DropTable(
                name: "MerchantActivitySetup");

            migrationBuilder.DropTable(
                name: "MerchantBankInfo");

            migrationBuilder.DropTable(
                name: "MerchantBureauSearch");

            migrationBuilder.DropTable(
                name: "MerchantBusinessInfo");

            migrationBuilder.DropTable(
                name: "MerchantQRCodeOnboardingResponse");

            migrationBuilder.DropTable(
                name: "MerchantStore");

            migrationBuilder.DropTable(
                name: "MerchantStoreRequest");

            migrationBuilder.DropTable(
                name: "MerchantTransactionSetup");

            migrationBuilder.DropTable(
                name: "MerchantWallet");

            migrationBuilder.DropTable(
                name: "NonEscrowFioranoT24Request");

            migrationBuilder.DropTable(
                name: "OnboardingNotiification");

            migrationBuilder.DropTable(
                name: "OtherMerchantBankInfo");

            migrationBuilder.DropTable(
                name: "PaymentResponse");

            migrationBuilder.DropTable(
                name: "PinRequest");

            migrationBuilder.DropTable(
                name: "productInventoryHistories");

            migrationBuilder.DropTable(
                name: "ProductItems");

            migrationBuilder.DropTable(
                name: "QrPaymentResponse");

            migrationBuilder.DropTable(
                name: "RequestTicketRequest");

            migrationBuilder.DropTable(
                name: "RequestTicketResponse");

            migrationBuilder.DropTable(
                name: "SendBvnPhoneVerificationCodeResponse");

            migrationBuilder.DropTable(
                name: "SendEmailVerificationCodeRequest");

            migrationBuilder.DropTable(
                name: "SendEmailVerificationCodeResponse");

            migrationBuilder.DropTable(
                name: "SendOtpRequest");

            migrationBuilder.DropTable(
                name: "SendOtpResponse");

            migrationBuilder.DropTable(
                name: "SendPhoneRequest");

            migrationBuilder.DropTable(
                name: "SendPhoneResponse");

            migrationBuilder.DropTable(
                name: "SendPinRequest");

            migrationBuilder.DropTable(
                name: "SendPinResponse");

            migrationBuilder.DropTable(
                name: "SetDisbursementAccountRequest");

            migrationBuilder.DropTable(
                name: "SetDisbursementAccountResponse");

            migrationBuilder.DropTable(
                name: "SingleDstvPaymentResponse");

            migrationBuilder.DropTable(
                name: "SpectaRegisterCustomerResponse");

            migrationBuilder.DropTable(
                name: "SpectaStageVerificationPinRequest");

            migrationBuilder.DropTable(
                name: "StoreTransactionLogDetails");

            migrationBuilder.DropTable(
                name: "SubMerchantQRCodeOnboardingResponse");

            migrationBuilder.DropTable(
                name: "TenantProfile");

            migrationBuilder.DropTable(
                name: "TransactionLog");

            migrationBuilder.DropTable(
                name: "UssdServiceRequestLog");

            migrationBuilder.DropTable(
                name: "ValidateChargeRequest");

            migrationBuilder.DropTable(
                name: "ValidateChargeResponse");

            migrationBuilder.DropTable(
                name: "VendAirtimeRequestLog");

            migrationBuilder.DropTable(
                name: "VerifyBvnPhoneConfirmationCodeRequest");

            migrationBuilder.DropTable(
                name: "VerifyBvnPhoneConfirmationCodeResponse");

            migrationBuilder.DropTable(
                name: "VerifyEmailConfirmationCodeRequest");

            migrationBuilder.DropTable(
                name: "VerifyEmailConfirmationCodeResponse");

            migrationBuilder.DropTable(
                name: "WalletTransferRequestLog");

            migrationBuilder.DropTable(
                name: "WalletTransferResponse");

            migrationBuilder.DropTable(
                name: "WebHookRequest");

            migrationBuilder.DropTable(
                name: "WebHookTransactionRequestLog");

            migrationBuilder.DropTable(
                name: "BindMerchant");

            migrationBuilder.DropTable(
                name: "MerchantPaymentSetup");

            migrationBuilder.DropTable(
                name: "DstvAccountLookup");

            migrationBuilder.DropTable(
                name: "FioranoBillsRequest");

            migrationBuilder.DropTable(
                name: "InvoicePaymentLink");

            migrationBuilder.DropTable(
                name: "ApplyForLoan");

            migrationBuilder.DropTable(
                name: "ProductOption");

            migrationBuilder.DropTable(
                name: "StoreCategory");

            migrationBuilder.DropTable(
                name: "ProductInventory");

            migrationBuilder.DropTable(
                name: "QrPaymentRequest");

            migrationBuilder.DropTable(
                name: "SingleDstvPayment");

            migrationBuilder.DropTable(
                name: "SpectaRegisterCustomerRequest");

            migrationBuilder.DropTable(
                name: "StoreTransactionLog");

            migrationBuilder.DropTable(
                name: "SubMerchantQRCodeOnboarding");

            migrationBuilder.DropTable(
                name: "LoanRepaymentPlan");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "MerchantQRCodeOnboarding");

            migrationBuilder.DropTable(
                name: "InventoryHistory");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "PurchasedProduct");

            migrationBuilder.DropTable(
                name: "ClientAuthentication");
        }
    }
}
