using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Taxpayers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SocialSecurityNumber = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    TaxIdentificationNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FilingStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AnnualIncome = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EmploymentStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EmployerName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EmployerIdentificationNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TaxYear = table.Column<int>(type: "integer", nullable: false),
                    HasFiledTaxes = table.Column<bool>(type: "boolean", nullable: false),
                    TaxFilingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TaxLiability = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TaxPaid = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TaxRefund = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taxpayers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxpayerAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreetAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ZipCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AddressType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TaxpayerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxpayerAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxpayerAddresses_Taxpayers_TaxpayerId",
                        column: x => x.TaxpayerId,
                        principalTable: "Taxpayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaxpayerAddresses_IsPrimary",
                table: "TaxpayerAddresses",
                column: "IsPrimary");

            migrationBuilder.CreateIndex(
                name: "IX_TaxpayerAddresses_TaxpayerId",
                table: "TaxpayerAddresses",
                column: "TaxpayerId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxpayerAddresses_TaxpayerId_IsPrimary",
                table: "TaxpayerAddresses",
                columns: new[] { "TaxpayerId", "IsPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_Taxpayers_CreatedDate",
                table: "Taxpayers",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Taxpayers_Email",
                table: "Taxpayers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Taxpayers_IsActive",
                table: "Taxpayers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Taxpayers_SocialSecurityNumber",
                table: "Taxpayers",
                column: "SocialSecurityNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Taxpayers_TaxIdentificationNumber",
                table: "Taxpayers",
                column: "TaxIdentificationNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaxpayerAddresses");

            migrationBuilder.DropTable(
                name: "Taxpayers");
        }
    }
}
