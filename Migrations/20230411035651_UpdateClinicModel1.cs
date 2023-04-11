using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValenciaBot.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClinicModel1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocationDescription",
                table: "Clinics",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Clinics",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "WhatsappNumber",
                table: "Clinics",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationDescription",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "WhatsappNumber",
                table: "Clinics");
        }
    }
}
