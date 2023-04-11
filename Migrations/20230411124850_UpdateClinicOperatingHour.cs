using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValenciaBot.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClinicOperatingHour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DaysDescription",
                table: "ServiceOperatingHours",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DaysDescription",
                table: "ClinicOperatingHours",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysDescription",
                table: "ServiceOperatingHours");

            migrationBuilder.DropColumn(
                name: "DaysDescription",
                table: "ClinicOperatingHours");
        }
    }
}
