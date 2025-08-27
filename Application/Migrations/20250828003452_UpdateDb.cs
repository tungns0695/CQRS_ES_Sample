using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LastEventPosition",
                table: "Taxpayers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastEventPosition",
                table: "TaxpayerAddresses",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastEventPosition",
                table: "Taxpayers");

            migrationBuilder.DropColumn(
                name: "LastEventPosition",
                table: "TaxpayerAddresses");
        }
    }
}
