using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCheckpoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LastEventId",
                table: "Taxpayers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Taxpayers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "LastEventId",
                table: "TaxpayerAddresses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "TaxpayerAddresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AppliedEvents",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Position = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppliedEvents", x => new { x.EventId, x.ProjectorName });
                });

            migrationBuilder.CreateTable(
                name: "ProjectorCheckpoints",
                columns: table => new
                {
                    ProjectorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LastPosition = table.Column<long>(type: "bigint", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectorCheckpoints", x => x.ProjectorName);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppliedEvents_Position",
                table: "AppliedEvents",
                column: "Position");

            migrationBuilder.CreateIndex(
                name: "IX_AppliedEvents_ProjectorName",
                table: "AppliedEvents",
                column: "ProjectorName");

            migrationBuilder.CreateIndex(
                name: "IX_AppliedEvents_ProjectorName_Position",
                table: "AppliedEvents",
                columns: new[] { "ProjectorName", "Position" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppliedEvents");

            migrationBuilder.DropTable(
                name: "ProjectorCheckpoints");

            migrationBuilder.DropColumn(
                name: "LastEventId",
                table: "Taxpayers");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Taxpayers");

            migrationBuilder.DropColumn(
                name: "LastEventId",
                table: "TaxpayerAddresses");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "TaxpayerAddresses");
        }
    }
}
