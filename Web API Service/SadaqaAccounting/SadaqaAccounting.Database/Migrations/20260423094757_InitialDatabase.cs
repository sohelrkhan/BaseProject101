using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SadaqaAccounting.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MasterSettings");

            migrationBuilder.EnsureSchema(
                name: "AssetManagement");

            migrationBuilder.EnsureSchema(
                name: "Cash&BankManagement");

            migrationBuilder.EnsureSchema(
                name: "DonorManagement");

            migrationBuilder.EnsureSchema(
                name: "ExpenseManagement");

            migrationBuilder.EnsureSchema(
                name: "IncomeManagement");

            migrationBuilder.CreateTable(
                name: "AccountUnits",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnumTypes",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnumTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLoginHistories",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoginIp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoginDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LogoutDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cashes",
                schema: "Cash&BankManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountUnitId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cashes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cashes_AccountUnits_AccountUnitId",
                        column: x => x.AccountUnitId,
                        principalSchema: "MasterSettings",
                        principalTable: "AccountUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountUnitId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_AccountUnits_AccountUnitId",
                        column: x => x.AccountUnitId,
                        principalSchema: "MasterSettings",
                        principalTable: "AccountUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                schema: "ExpenseManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountUnitId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 50, nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseCategories_AccountUnits_AccountUnitId",
                        column: x => x.AccountUnitId,
                        principalSchema: "MasterSettings",
                        principalTable: "AccountUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncomeCategories",
                schema: "IncomeManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountUnitId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 50, nullable: false),
                    IsDonorBased = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeCategories_AccountUnits_AccountUnitId",
                        column: x => x.AccountUnitId,
                        principalSchema: "MasterSettings",
                        principalTable: "AccountUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EnumTypeCollections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EnumTypeId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnumTypeCollections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnumTypeCollections_EnumTypes_EnumTypeId",
                        column: x => x.EnumTypeId,
                        principalSchema: "MasterSettings",
                        principalTable: "EnumTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Actions",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actions_EnumTypeCollections_StatusId",
                        column: x => x.StatusId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                schema: "AssetManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountUnitId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 50, nullable: false),
                    AssetNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchaseValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_AccountUnits_AccountUnitId",
                        column: x => x.AccountUnitId,
                        principalSchema: "MasterSettings",
                        principalTable: "AccountUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assets_EnumTypeCollections_StatusId",
                        column: x => x.StatusId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Banks",
                schema: "Cash&BankManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountUnitId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 50, nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(100)", maxLength: 50, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 25, nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banks_AccountUnits_AccountUnitId",
                        column: x => x.AccountUnitId,
                        principalSchema: "MasterSettings",
                        principalTable: "AccountUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Banks_EnumTypeCollections_StatusId",
                        column: x => x.StatusId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CashLedgers",
                schema: "Cash&BankManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountUnitId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionTypeId = table.Column<int>(type: "int", nullable: false),
                    SourceTypeId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    CashId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashLedgers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashLedgers_AccountUnits_AccountUnitId",
                        column: x => x.AccountUnitId,
                        principalSchema: "MasterSettings",
                        principalTable: "AccountUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashLedgers_Cashes_CashId",
                        column: x => x.CashId,
                        principalSchema: "Cash&BankManagement",
                        principalTable: "Cashes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashLedgers_EnumTypeCollections_SourceTypeId",
                        column: x => x.SourceTypeId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashLedgers_EnumTypeCollections_TransactionTypeId",
                        column: x => x.TransactionTypeId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 50, nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_EnumTypeCollections_StatusId",
                        column: x => x.StatusId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Donors",
                schema: "DonorManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountUnitId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 50, nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Donors_AccountUnits_AccountUnitId",
                        column: x => x.AccountUnitId,
                        principalSchema: "MasterSettings",
                        principalTable: "AccountUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Donors_EnumTypeCollections_StatusId",
                        column: x => x.StatusId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modules_EnumTypeCollections_StatusId",
                        column: x => x.StatusId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_EnumTypeCollections_StatusId",
                        column: x => x.StatusId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankLedgers",
                schema: "Cash&BankManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountUnitId = table.Column<int>(type: "int", nullable: false),
                    BankId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionTypeId = table.Column<int>(type: "int", nullable: false),
                    SourceTypeId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankLedgers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankLedgers_AccountUnits_AccountUnitId",
                        column: x => x.AccountUnitId,
                        principalSchema: "MasterSettings",
                        principalTable: "AccountUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankLedgers_Banks_BankId",
                        column: x => x.BankId,
                        principalSchema: "Cash&BankManagement",
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankLedgers_EnumTypeCollections_SourceTypeId",
                        column: x => x.SourceTypeId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankLedgers_EnumTypeCollections_TransactionTypeId",
                        column: x => x.TransactionTypeId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                schema: "ExpenseManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountUnitId = table.Column<int>(type: "int", nullable: false),
                    ExpenseCategoryId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    ExpenseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentModeId = table.Column<int>(type: "int", nullable: false),
                    BankId = table.Column<int>(type: "int", nullable: true),
                    CashId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_AccountUnits_AccountUnitId",
                        column: x => x.AccountUnitId,
                        principalSchema: "MasterSettings",
                        principalTable: "AccountUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expenses_Banks_BankId",
                        column: x => x.BankId,
                        principalSchema: "Cash&BankManagement",
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expenses_Cashes_CashId",
                        column: x => x.CashId,
                        principalSchema: "Cash&BankManagement",
                        principalTable: "Cashes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expenses_EnumTypeCollections_PaymentModeId",
                        column: x => x.PaymentModeId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expenses_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "MasterSettings",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                        column: x => x.ExpenseCategoryId,
                        principalSchema: "ExpenseManagement",
                        principalTable: "ExpenseCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OpeningBalances",
                schema: "Cash&BankManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountUnitId = table.Column<int>(type: "int", nullable: false),
                    PaymentModeId = table.Column<int>(type: "int", nullable: false),
                    BankId = table.Column<int>(type: "int", nullable: true),
                    CashId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OpeningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpeningBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpeningBalances_AccountUnits_AccountUnitId",
                        column: x => x.AccountUnitId,
                        principalSchema: "MasterSettings",
                        principalTable: "AccountUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpeningBalances_Banks_BankId",
                        column: x => x.BankId,
                        principalSchema: "Cash&BankManagement",
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpeningBalances_Cashes_CashId",
                        column: x => x.CashId,
                        principalSchema: "Cash&BankManagement",
                        principalTable: "Cashes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpeningBalances_EnumTypeCollections_PaymentModeId",
                        column: x => x.PaymentModeId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "MasterSettings",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_EnumTypeCollections_StatusId",
                        column: x => x.StatusId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Incomes",
                schema: "IncomeManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountUnitId = table.Column<int>(type: "int", nullable: false),
                    IncomeCategoryId = table.Column<int>(type: "int", nullable: false),
                    DonorId = table.Column<int>(type: "int", nullable: true),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    CashId = table.Column<int>(type: "int", nullable: true),
                    ReceiptBookNo = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    ReceiptNo = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentModeId = table.Column<int>(type: "int", nullable: false),
                    BankId = table.Column<int>(type: "int", nullable: true),
                    MonthId = table.Column<int>(type: "int", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incomes_AccountUnits_AccountUnitId",
                        column: x => x.AccountUnitId,
                        principalSchema: "MasterSettings",
                        principalTable: "AccountUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incomes_Banks_BankId",
                        column: x => x.BankId,
                        principalSchema: "Cash&BankManagement",
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incomes_Cashes_CashId",
                        column: x => x.CashId,
                        principalSchema: "Cash&BankManagement",
                        principalTable: "Cashes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incomes_Donors_DonorId",
                        column: x => x.DonorId,
                        principalSchema: "DonorManagement",
                        principalTable: "Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incomes_EnumTypeCollections_MonthId",
                        column: x => x.MonthId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incomes_EnumTypeCollections_PaymentModeId",
                        column: x => x.PaymentModeId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Incomes_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "MasterSettings",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incomes_IncomeCategories_IncomeCategoryId",
                        column: x => x.IncomeCategoryId,
                        principalSchema: "IncomeManagement",
                        principalTable: "IncomeCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 500, nullable: false),
                    LinkedTableName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LinkedControllerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Features_EnumTypeCollections_StatusId",
                        column: x => x.StatusId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Features_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "MasterSettings",
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportRegistries",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    ReportCode = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ReportGroup = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportRegistries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportRegistries_EnumTypeCollections_StatusId",
                        column: x => x.StatusId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportRegistries_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "MasterSettings",
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    ForcePasswordChanged = table.Column<bool>(type: "bit", nullable: true),
                    ApplicationUserTypeId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "MasterSettings",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_EnumTypeCollections_ApplicationUserTypeId",
                        column: x => x.ApplicationUserTypeId,
                        principalTable: "EnumTypeCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeatureActionMappings",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeatureId = table.Column<int>(type: "int", nullable: false),
                    ActionId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureActionMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeatureActionMappings_Actions_ActionId",
                        column: x => x.ActionId,
                        principalSchema: "MasterSettings",
                        principalTable: "Actions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeatureActionMappings_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalSchema: "MasterSettings",
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleActionMappings",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    FeatureId = table.Column<int>(type: "int", nullable: false),
                    ActionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleActionMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleActionMappings_Actions_ActionId",
                        column: x => x.ActionId,
                        principalSchema: "MasterSettings",
                        principalTable: "Actions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleActionMappings_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalSchema: "MasterSettings",
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleActionMappings_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "MasterSettings",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportUserAccess",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportRegistryId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportUserAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportUserAccess_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportUserAccess_ReportRegistries_ReportRegistryId",
                        column: x => x.ReportRegistryId,
                        principalSchema: "MasterSettings",
                        principalTable: "ReportRegistries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAccessMappings",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FeatureId = table.Column<int>(type: "int", nullable: false),
                    ActionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccessMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAccessMappings_Actions_ActionId",
                        column: x => x.ActionId,
                        principalSchema: "MasterSettings",
                        principalTable: "Actions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAccessMappings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAccessMappings_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalSchema: "MasterSettings",
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAccountUnits",
                schema: "MasterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountUnitId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccountUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAccountUnits_AccountUnits_AccountUnitId",
                        column: x => x.AccountUnitId,
                        principalSchema: "MasterSettings",
                        principalTable: "AccountUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAccountUnits_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actions_StatusId",
                schema: "MasterSettings",
                table: "Actions",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ApplicationUserTypeId",
                table: "AspNetUsers",
                column: "ApplicationUserTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmployeeId",
                table: "AspNetUsers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AccountUnitId",
                schema: "AssetManagement",
                table: "Assets",
                column: "AccountUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_StatusId",
                schema: "AssetManagement",
                table: "Assets",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_BankLedgers_AccountUnitId",
                schema: "Cash&BankManagement",
                table: "BankLedgers",
                column: "AccountUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_BankLedgers_BankId",
                schema: "Cash&BankManagement",
                table: "BankLedgers",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_BankLedgers_SourceTypeId",
                schema: "Cash&BankManagement",
                table: "BankLedgers",
                column: "SourceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BankLedgers_TransactionTypeId",
                schema: "Cash&BankManagement",
                table: "BankLedgers",
                column: "TransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_AccountUnitId",
                schema: "Cash&BankManagement",
                table: "Banks",
                column: "AccountUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_StatusId",
                schema: "Cash&BankManagement",
                table: "Banks",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Cashes_AccountUnitId",
                schema: "Cash&BankManagement",
                table: "Cashes",
                column: "AccountUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_CashLedgers_AccountUnitId",
                schema: "Cash&BankManagement",
                table: "CashLedgers",
                column: "AccountUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_CashLedgers_CashId",
                schema: "Cash&BankManagement",
                table: "CashLedgers",
                column: "CashId");

            migrationBuilder.CreateIndex(
                name: "IX_CashLedgers_SourceTypeId",
                schema: "Cash&BankManagement",
                table: "CashLedgers",
                column: "SourceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CashLedgers_TransactionTypeId",
                schema: "Cash&BankManagement",
                table: "CashLedgers",
                column: "TransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_StatusId",
                schema: "MasterSettings",
                table: "Companies",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Donors_AccountUnitId",
                schema: "DonorManagement",
                table: "Donors",
                column: "AccountUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Donors_StatusId",
                schema: "DonorManagement",
                table: "Donors",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId",
                schema: "MasterSettings",
                table: "Employees",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_StatusId",
                schema: "MasterSettings",
                table: "Employees",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EnumTypeCollections_EnumTypeId",
                table: "EnumTypeCollections",
                column: "EnumTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_AccountUnitId",
                schema: "MasterSettings",
                table: "Events",
                column: "AccountUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_AccountUnitId",
                schema: "ExpenseManagement",
                table: "ExpenseCategories",
                column: "AccountUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_AccountUnitId",
                schema: "ExpenseManagement",
                table: "Expenses",
                column: "AccountUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BankId",
                schema: "ExpenseManagement",
                table: "Expenses",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CashId",
                schema: "ExpenseManagement",
                table: "Expenses",
                column: "CashId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_EventId",
                schema: "ExpenseManagement",
                table: "Expenses",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseCategoryId",
                schema: "ExpenseManagement",
                table: "Expenses",
                column: "ExpenseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PaymentModeId",
                schema: "ExpenseManagement",
                table: "Expenses",
                column: "PaymentModeId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureActionMappings_ActionId",
                schema: "MasterSettings",
                table: "FeatureActionMappings",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureActionMappings_FeatureId",
                schema: "MasterSettings",
                table: "FeatureActionMappings",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Features_ModuleId",
                schema: "MasterSettings",
                table: "Features",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Features_StatusId",
                schema: "MasterSettings",
                table: "Features",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeCategories_AccountUnitId",
                schema: "IncomeManagement",
                table: "IncomeCategories",
                column: "AccountUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_AccountUnitId",
                schema: "IncomeManagement",
                table: "Incomes",
                column: "AccountUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_BankId",
                schema: "IncomeManagement",
                table: "Incomes",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_CashId",
                schema: "IncomeManagement",
                table: "Incomes",
                column: "CashId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_DonorId",
                schema: "IncomeManagement",
                table: "Incomes",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_EventId",
                schema: "IncomeManagement",
                table: "Incomes",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_IncomeCategoryId",
                schema: "IncomeManagement",
                table: "Incomes",
                column: "IncomeCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_MonthId",
                schema: "IncomeManagement",
                table: "Incomes",
                column: "MonthId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_PaymentModeId",
                schema: "IncomeManagement",
                table: "Incomes",
                column: "PaymentModeId");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_StatusId",
                schema: "MasterSettings",
                table: "Modules",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningBalances_AccountUnitId",
                schema: "Cash&BankManagement",
                table: "OpeningBalances",
                column: "AccountUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningBalances_BankId",
                schema: "Cash&BankManagement",
                table: "OpeningBalances",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningBalances_CashId",
                schema: "Cash&BankManagement",
                table: "OpeningBalances",
                column: "CashId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningBalances_PaymentModeId",
                schema: "Cash&BankManagement",
                table: "OpeningBalances",
                column: "PaymentModeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRegistries_ModuleId",
                schema: "MasterSettings",
                table: "ReportRegistries",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRegistries_StatusId",
                schema: "MasterSettings",
                table: "ReportRegistries",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportUserAccess_ReportRegistryId",
                schema: "MasterSettings",
                table: "ReportUserAccess",
                column: "ReportRegistryId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportUserAccess_UserId",
                schema: "MasterSettings",
                table: "ReportUserAccess",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleActionMappings_ActionId",
                schema: "MasterSettings",
                table: "RoleActionMappings",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleActionMappings_FeatureId",
                schema: "MasterSettings",
                table: "RoleActionMappings",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleActionMappings_RoleId",
                schema: "MasterSettings",
                table: "RoleActionMappings",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_StatusId",
                schema: "MasterSettings",
                table: "Roles",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccessMappings_ActionId",
                schema: "MasterSettings",
                table: "UserAccessMappings",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccessMappings_FeatureId",
                schema: "MasterSettings",
                table: "UserAccessMappings",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccessMappings_UserId",
                schema: "MasterSettings",
                table: "UserAccessMappings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccountUnits_AccountUnitId",
                schema: "MasterSettings",
                table: "UserAccountUnits",
                column: "AccountUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccountUnits_UserId",
                schema: "MasterSettings",
                table: "UserAccountUnits",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Assets",
                schema: "AssetManagement");

            migrationBuilder.DropTable(
                name: "BankLedgers",
                schema: "Cash&BankManagement");

            migrationBuilder.DropTable(
                name: "CashLedgers",
                schema: "Cash&BankManagement");

            migrationBuilder.DropTable(
                name: "Expenses",
                schema: "ExpenseManagement");

            migrationBuilder.DropTable(
                name: "FeatureActionMappings",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "Incomes",
                schema: "IncomeManagement");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "OpeningBalances",
                schema: "Cash&BankManagement");

            migrationBuilder.DropTable(
                name: "ReportUserAccess",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "RoleActionMappings",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "UserAccessMappings",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "UserAccountUnits",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "UserLoginHistories",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ExpenseCategories",
                schema: "ExpenseManagement");

            migrationBuilder.DropTable(
                name: "Donors",
                schema: "DonorManagement");

            migrationBuilder.DropTable(
                name: "Events",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "IncomeCategories",
                schema: "IncomeManagement");

            migrationBuilder.DropTable(
                name: "Banks",
                schema: "Cash&BankManagement");

            migrationBuilder.DropTable(
                name: "Cashes",
                schema: "Cash&BankManagement");

            migrationBuilder.DropTable(
                name: "ReportRegistries",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "Actions",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "Features",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AccountUnits",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "Modules",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "Employees",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "MasterSettings");

            migrationBuilder.DropTable(
                name: "EnumTypeCollections");

            migrationBuilder.DropTable(
                name: "EnumTypes",
                schema: "MasterSettings");
        }
    }
}
