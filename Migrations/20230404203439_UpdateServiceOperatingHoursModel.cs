using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValenciaBot.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServiceOperatingHoursModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOperatingHours_ClinicServices_ClinicServiceId",
                table: "ServiceOperatingHours");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOperatingHours_Services_ServiceId",
                table: "ServiceOperatingHours");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOperatingHours_ClinicServiceId",
                table: "ServiceOperatingHours");

            migrationBuilder.DropColumn(
                name: "ClinicServiceId",
                table: "ServiceOperatingHours");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOperatingHours_ClinicServices_ServiceId",
                table: "ServiceOperatingHours",
                column: "ServiceId",
                principalTable: "ClinicServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOperatingHours_ClinicServices_ServiceId",
                table: "ServiceOperatingHours");

            migrationBuilder.AddColumn<int>(
                name: "ClinicServiceId",
                table: "ServiceOperatingHours",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOperatingHours_ClinicServiceId",
                table: "ServiceOperatingHours",
                column: "ClinicServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOperatingHours_ClinicServices_ClinicServiceId",
                table: "ServiceOperatingHours",
                column: "ClinicServiceId",
                principalTable: "ClinicServices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOperatingHours_Services_ServiceId",
                table: "ServiceOperatingHours",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
