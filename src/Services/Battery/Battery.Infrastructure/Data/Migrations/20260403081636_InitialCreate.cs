using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Battery.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BatteryStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Percentage = table.Column<int>(type: "int", nullable: false),
                    Voltage = table.Column<double>(type: "float", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: true),
                    IsCharging = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryStatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BatteryStatuses_Timestamp",
                table: "BatteryStatuses",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryStatuses_VehicleId",
                table: "BatteryStatuses",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatteryStatuses");
        }
    }
}
