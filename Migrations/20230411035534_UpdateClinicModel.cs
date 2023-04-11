using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValenciaBot.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClinicModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lattitude",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Clinics");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Clinics",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Clinics");

            migrationBuilder.AddColumn<string>(
                name: "Lattitude",
                table: "Clinics",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "Clinics",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
